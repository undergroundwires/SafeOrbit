using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafeOrbit.Common;
using SafeOrbit.Library;
using SafeOrbit.Threading;
using SafeOrbit.Memory.SafeStringServices.Text;
using Encoding = SafeOrbit.Memory.SafeStringServices.Text.Encoding;

namespace SafeOrbit.Memory
{
    public class SafeString : DisposableBase, ISafeString
    {
        private const Encoding InnerEncoding = Encoding.Utf16LittleEndian;
        private const byte LineFeed = 0x000A; //http://www.fileformat.info/info/unicode/char/000A/index.htm
        private readonly IList<ISafeBytes> _charBytesList;
        private readonly IFactory<ISafeBytes> _safeBytesFactory;
        private readonly IFactory<ISafeString> _safeStringFactory;
        private readonly ITextService _textService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SafeString" /> class.
        /// </summary>
        public SafeString() : this(SafeOrbitCore.Current.Factory.Get<ITextService>(),
            SafeContainerWrapper<ISafeString>.Wrap(),
            SafeContainerWrapper<ISafeBytes>.Wrap())
        {
        }

        internal SafeString(
            ITextService textService,
            IFactory<ISafeString> safeStringFactory,
            IFactory<ISafeBytes> safeBytesFactory)
        {
            _textService = textService ?? throw new ArgumentNullException(nameof(textService));
            _safeStringFactory = safeStringFactory ?? throw new ArgumentNullException(nameof(safeStringFactory));
            _safeBytesFactory = safeBytesFactory ?? throw new ArgumentNullException(nameof(safeBytesFactory));
            _charBytesList = new List<ISafeBytes>();
        }

        public int Length => _charBytesList == null || IsDisposed ? 0 : _charBytesList.Count;
        public bool IsEmpty => Length == 0;

        /// <inheritdoc />
        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        public Task AppendAsync(char ch) => InsertAsync(Length, ch);

        /// <inheritdoc />
        /// <param name="text">Non-safe <see cref="T:System.String" /> that's already revealed in the memory</param>
        /// <exception cref="T:System.ArgumentNullException"> <paramref name="text" /> is <see langword="null" />.</exception>
        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        public async Task AppendAsync(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            ThrowIfDisposed();
            if (text.Length == 0)
                return;
            foreach (var ch in text)
            {
                await AppendAsync(ch).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="character" /> is <see langword="null" />. </exception>
        /// <exception cref="ArgumentOutOfRangeException">If position is less than zero or higher than the length.  </exception>
        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        public Task AppendAsync(ISafeBytes character, Encoding encoding = Encoding.Utf16LittleEndian)
        {
            return InsertAsync(Length, character, encoding);
        }

        /// <exception cref="ArgumentNullException"><paramref name="safeString" /> is <see langword="null" />. </exception>
        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        public async Task AppendAsync(ISafeString safeString)
        {
            if (safeString == null) throw new ArgumentNullException(nameof(safeString));
            ThrowIfDisposed();
            for (var i = 0; i < safeString.Length; i++)
            {
                var @byte = await safeString.GetAsSafeBytes(i).DeepCloneAsync().ConfigureAwait(false);
                await AppendAsync(@byte).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        public async Task AppendLineAsync()
        {
            ThrowIfDisposed();
            var safeBytes = _safeBytesFactory.Create();
            await safeBytes.AppendAsync(LineFeed).ConfigureAwait(false);
            await AppendAsync(safeBytes, InnerEncoding).ConfigureAwait(false);
        }

        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index.</exception>
        public async Task InsertAsync(int index, char character)
        {
            if (index < 0 || index > Length) throw new ArgumentOutOfRangeException(nameof(index));
            ThrowIfDisposed();
            var bytes = await TransformCharToSafeBytesAsync(character, InnerEncoding)
                .ConfigureAwait(false);
            _charBytesList.Insert(index, bytes);
        }

        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        /// <exception cref="ArgumentNullException"> <paramref name="character" /> is <see langword="null" />. </exception>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="character" /> is null or empty. </exception>
        public async Task InsertAsync(int position, ISafeBytes character, Encoding encoding = Encoding.Utf16LittleEndian)
        {
            ThrowIfDisposed();
            if (position < 0 || position > Length) throw new ArgumentOutOfRangeException(nameof(position));
            if (SafeBytes.IsNullOrEmpty(character)) throw new ArgumentNullException(nameof(character));
            if (encoding != InnerEncoding)
            {
                character = await ConvertEncodingAsync(character, encoding, InnerEncoding)
                    .ConfigureAwait(false);
            }
            _charBytesList.Insert(position, character);
        }
        public static Task ForEachAsync<T>(IEnumerable<T> source, int degreeOfParalellism, Func<T, Task> action)
        {
            return Task.WhenAll(Partitioner.Create(source).GetPartitions(degreeOfParalellism).Select(partition => Task.Run(async () =>
            {
                using (partition)
                    while (partition.MoveNext())
                        await action(partition.Current);
            })));
        }
        public async Task<byte[]> RevealDecryptedBytesAsync()
        {
            ThrowIfDisposed();
            if (IsEmpty) return new byte[0];
            var tasks = new Task<byte[]>[_charBytesList.Count];
            for (var i = 0; i < tasks.Length; i++)
                tasks[i] = _charBytesList[i].RevealDecryptedBytesAsync();
            await Task.WhenAll(tasks).ConfigureAwait(false);
            var result = new byte[_charBytesList.Select(l => l.Length).Sum()];
            var currentByteCount = 0;
            foreach (var block in tasks.Select(t=> t.Result))
            {
                Buffer.BlockCopy(block, 0, result, currentByteCount, block.Length);
                currentByteCount += block.Length;
            }
            return result;
        }

        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        /// <exception cref="InvalidOperationException"><see cref="SafeString" /> instance is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="index" /> is not a valid index. </exception>
        public ISafeBytes GetAsSafeBytes(int index)
        {
            ThrowIfDisposed();
            EnsureNotEmpty();
            if (index < 0 || index >= Length) throw new ArgumentOutOfRangeException(nameof(index));
            var result = _charBytesList.ElementAt(index);
            return result;
        }

        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        /// <exception cref="InvalidOperationException"><see cref="SafeString" /> instance is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="index" /> is less than zero, higher than/equals to the
        ///     length.
        /// </exception>
        public async Task<char> RevealDecryptedCharAsync(int index)
        {
            ThrowIfDisposed();
            EnsureNotEmpty();
            if (index < 0 || index >= Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            var asSafeBytes = _charBytesList.ElementAt(index);
            var asChar = await TransformSafeBytesToCharAsync(asSafeBytes, InnerEncoding)
                .ConfigureAwait(false);
            return asChar;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <p><paramref name="startIndex" /> is out of range.</p>
        ///     <p><paramref name="count" /> is less than one.</p>
        ///     <p>The total of <paramref name="startIndex" /> and <paramref name="count" /> is higher than length.</p>
        /// </exception>
        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        /// <exception cref="InvalidOperationException"> The <see cref="SafeString" /> instance is empty. </exception>
        public void Remove(int startIndex, int count = 1)
        {
            ThrowIfDisposed();
            EnsureNotEmpty();
            if (startIndex < 0 || startIndex >= Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (startIndex + count > Length)
                throw new ArgumentOutOfRangeException(
                    $"Total of {nameof(count)}({count}) and {nameof(startIndex)}({startIndex} is higher than the length({Length})");
            for (var i = 0; i < count; i++)
            {
                _charBytesList[startIndex].Dispose();
                _charBytesList[startIndex] = null;
                _charBytesList.RemoveAt(startIndex);
            }
        }

        /// <inheritdoc />
        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        public void Clear()
        {
            ThrowIfDisposed();
            for (var i = 0; i < Length; i++)
                Remove(i);
            _charBytesList.Clear();
        }
        
        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        public override int GetHashCode()
        {
            ThrowIfDisposed();
            unchecked
            {
                const int multiplier = 69;
                var hashCode = 0x2D2816FE;
                for (var i = 0; i < Length; i++)
                    hashCode *= multiplier + _charBytesList[i].GetHashCode();
                return hashCode;
            }
        }

        /// <exception cref="NotSupportedException"> Use <see cref="EqualsAsync(string)"/> or <see cref="EqualsAsync(ISafeString)"/> instead. </exception>
        public override bool Equals(object obj)
        {
            throw new NotSupportedException($"Use {nameof(EqualsAsync)} instead");
        }

        /// <exception cref="ArgumentNullException"> <paramref name="other" /> is <see langword="null" />. </exception>
        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        public async Task<bool> EqualsAsync(string other)
        {
            ThrowIfDisposed();
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other.Length == 0)
                return Length == 0;
            var comparisonBit = (uint)Length ^ (uint)other.Length; // Caution: Don't check length first and then fall out, since that leaks length info
            for (var i = 0; i < Length && i < other.Length; i++)
            {
                var @char = other[i];
                var bytes = _textService.GetBytes(@char, InnerEncoding);
                var hasSameCharBytes = await GetAsSafeBytes(i).EqualsAsync(bytes)
                    .ConfigureAwait(false);
                comparisonBit |= (uint)(hasSameCharBytes ? 0 : 1);
            }
            return comparisonBit == 0;
        }

        /// <exception cref="ArgumentNullException"> <paramref name="other" /> is <see langword="null" />. </exception>
        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        public async Task<bool> EqualsAsync(ISafeString other)
        {
            ThrowIfDisposed();
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (other.Length == 0)
                return Length == 0;
            if (this.GetHashCode() != other.GetHashCode()) return false;
            // Caution: Don't check length first and then fall out, since that leaks length info
            var comparisonBit = (uint)Length ^ (uint)other.Length;
            for (var i = 0; i < Length && i < other.Length; i++)
            {
                var ownBytes = GetAsSafeBytes(i);
                var otherBytes = other.GetAsSafeBytes(i);
                var areSameBytes = await ownBytes.EqualsAsync(otherBytes)
                    .ConfigureAwait(false);
                comparisonBit |= (uint)(areSameBytes ? 0 : 1);
            }
            return comparisonBit == 0;
        }

        /// <inheritdoc />
        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        public async Task<ISafeString> DeepCloneAsync()
        {
            ThrowIfDisposed();
            var result = _safeStringFactory.Create();
            for (var i = 0; i < Length; i++)
            {
                var bytes = await GetAsSafeBytes(i).DeepCloneAsync()
                    .ConfigureAwait(false);
                await result.AppendAsync(bytes, InnerEncoding)
                    .ConfigureAwait(false);
            }
            return result;
        }

        /// <exception cref="ObjectDisposedException"><see cref="SafeString" /> instance is disposed.</exception>
        public ISafeString ShallowClone()
        {
            ThrowIfDisposed();
            // The MemberwiseClone method creates a shallow copy by creating a new object,
            // and then copying the nonstatic fields of the current object to the new object.
            // If a field is a value type, a bit - by - bit copy of the field is performed.
            // If a field is a reference type, the reference is copied but the referred object is not
            // therefore, the original object and its clone refer to the same object.
            // Documentation : https://msdn.microsoft.com/en-us/library/system.object.memberwiseclone.aspx
            return MemberwiseClone() as ISafeString;
        }


        public static bool IsNullOrEmpty(ISafeString safeString)
            => safeString == null || safeString.IsDisposed || safeString.IsEmpty;

        protected override void DisposeManagedResources()
        {
            foreach(var safeBytes in _charBytesList)
                safeBytes?.Dispose();
            this._charBytesList.Clear();
        }

        private async Task<ISafeBytes> ConvertEncodingAsync(ISafeBytes character, Encoding sourceEncoding, Encoding destinationEncoding)
        {

            var buffer = await character.RevealDecryptedBytesAsync().ConfigureAwait(false);
            try
            {
                buffer = _textService.Convert(sourceEncoding, destinationEncoding, buffer);
                var safeBytes = _safeBytesFactory.Create();
                var stream = new SafeMemoryStream(buffer);
                await safeBytes.AppendManyAsync(stream)
                    .ConfigureAwait(false);
                return safeBytes;
            }
            finally
            {
                Array.Clear(buffer, 0, buffer.Length);
            }
        }

        private async Task<char> TransformSafeBytesToCharAsync(ISafeBytes safeBytes, Encoding encoding)
        {
            var byteBuffer = await safeBytes.RevealDecryptedBytesAsync().ConfigureAwait(false);
            try
            {
                return _textService.GetChars(byteBuffer, encoding).First();
            }
            finally
            {
                Array.Clear(byteBuffer, 0, byteBuffer.Length);
            }
        }

        private async Task<ISafeBytes> TransformCharToSafeBytesAsync(char c, Encoding encoding)
        {
            var bytes = _textService.GetBytes(c, encoding);
            var stream = new SafeMemoryStream();
            await stream.WriteAsync(bytes, 0, bytes.Length)
                .ConfigureAwait(false);

            var safeBytes = _safeBytesFactory.Create();
            await safeBytes.AppendManyAsync(stream).ConfigureAwait(false);

            return safeBytes;
        }

        /// <exception cref="InvalidOperationException">Throws if the <see cref="SafeString" /> instance is empty.</exception>
        private void EnsureNotEmpty()
        {
            if (Length == 0)
                throw new InvalidOperationException($"{nameof(SafeString)} is empty");
        }

#if DEBUG
        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < Length; i++)
                sb.Append(TaskContext.RunSync(() => RevealDecryptedCharAsync(i)));
            return sb.ToString();
        }
#endif
    }
}