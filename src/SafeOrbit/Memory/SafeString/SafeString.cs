using System;
using System.Collections.Generic;
using System.Linq;
using SafeOrbit.Library;
using SafeOrbit.Text;

namespace SafeOrbit.Memory
{
    public class SafeString : ISafeString
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

        public int Length => (_charBytesList == null) || IsDisposed ? 0 : _charBytesList.Count;
        public bool IsEmpty => Length == 0;
        public bool IsDisposed { get; private set; }

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="SafeString" /> instance is disposed.</exception>
        public void Append(char ch)
        {
            Insert(Length, ch);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Appends a <see cref="T:System.String" /> that's already revealed in the memory.
        /// </summary>
        /// <param name="text">Non-safe <see cref="T:System.String" /> that's already revealed in the memory</param>
        /// <exception cref="T:System.ArgumentNullException"> <paramref name="text" /> is <see langword="null" />.</exception>
        public void Append(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            EnsureNotDisposed();
            if (text.Length == 0)
                return;
            foreach (var ch in text)
                Append(ch);
        }

        /// <inheritdoc />
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="character" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     If position is less than zero or higher than the length.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     Throws if the SafeString instance is disposed.
        /// </exception>
        public void Append(ISafeBytes character, Encoding encoding = Encoding.Utf16LittleEndian)
        {
            Insert(Length, character, encoding);
        }

        /// <exception cref="ArgumentNullException">
        ///     <paramref name="safeString" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     Throws if the SafeString instance is disposed.
        /// </exception>
        public void Append(ISafeString safeString)
        {
            if (safeString == null) throw new ArgumentNullException(nameof(safeString));
            EnsureNotDisposed();
            for (var i = 0; i < safeString.Length; i++)
                Append(safeString.GetAsSafeBytes(i).DeepClone());
        }

        /// <inheritdoc />
        /// <exception cref="T:System.ObjectDisposedException">Throws if the SafeString instance is disposed</exception>
        public void AppendLine()
        {
            EnsureNotDisposed();
            var safeBytes = _safeBytesFactory.Create();
            safeBytes.Append(LineFeed);
            Append(safeBytes, InnerEncoding);
        }

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="ISafeBytes" /> instance is disposed</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index.</exception>
        public void Insert(int index, char character)
        {
            if ((index < 0) || (index > Length)) throw new ArgumentOutOfRangeException(nameof(index));
            EnsureNotDisposed();
            var bytes = TransformCharToSafeBytes(character, InnerEncoding);
            _charBytesList.Insert(index, bytes);
        }

        /// <exception cref="ArgumentNullException">
        ///     <paramref name="textBytes" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     Throws if the SafeString instance is disposed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Throws if <paramref name="textBytes" /> is less than zero or higher than the length.
        /// </exception>
        public void Insert(int position, ISafeBytes textBytes, Encoding encoding = Encoding.Utf16LittleEndian)
        {
            EnsureNotDisposed();
            if ((position < 0) || (position > Length)) throw new ArgumentOutOfRangeException(nameof(position));
            if (SafeBytes.IsNullOrEmpty(textBytes)) throw new ArgumentNullException(nameof(textBytes));
            if (encoding == InnerEncoding)
            {
                _charBytesList.Insert(position, textBytes);
                return;
            }
            //Convert encoding
            var buffer = textBytes.ToByteArray();
            try
            {
                if(encoding != InnerEncoding)
                    buffer = _textService.Convert(encoding, InnerEncoding, buffer);
                var safeBytes = _safeBytesFactory.Create();
                for (var i = 0; i < buffer.Length; i++)
                {
                    //Append
                    safeBytes.Append(buffer[i]);
                    buffer[i] = 0x0;
                }
                _charBytesList.Insert(position, safeBytes);
            }
            finally
            {
                Array.Clear(buffer, 0, buffer.Length);
            }
        }

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="SafeString" /> instance is disposed.</exception>
        /// <exception cref="InvalidOperationException">Throws if the <see cref="SafeString" /> instance is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="index" /> is not a valid index.
        /// </exception>
        public ISafeBytes GetAsSafeBytes(int index)
        {
            EnsureNotDisposed();
            EnsureNotEmpty();
            if ((index < 0) || (index >= Length)) throw new ArgumentOutOfRangeException(nameof(index));
            var result = _charBytesList.ElementAt(index);
            return result;
        }

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="SafeString" /> instance is disposed.</exception>
        /// <exception cref="InvalidOperationException">Throws if the <see cref="SafeString" /> instance is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Throws if <paramref name="index" /> is less than zero, higher than/equals
        ///     to the length.
        /// </exception>
        public char GetAsChar(int index)
        {
            EnsureNotDisposed();
            EnsureNotEmpty();
            if ((index < 0) || (index >= Length))
                throw new ArgumentOutOfRangeException(nameof(index));
            var asSafeBytes = _charBytesList.ElementAt(index);
            var asChar = TransformSafeBytesToChar(asSafeBytes, InnerEncoding);
            return asChar;
        }

        /// <exception cref="ArgumentOutOfRangeException">
        ///     Throws when <paramref name="startIndex" /> is out of range.
        ///     Throws when <paramref name="count" /> is less than one.
        ///     Throws when the total of <paramref name="startIndex" /> and <paramref name="count" /> is higher than lenth.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     Throws if the <see cref="SafeString" /> instance is disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Throws if the <see cref="SafeString" /> instance is empty.
        /// </exception>
        public void Remove(int startIndex, int count = 1)
        {
            EnsureNotDisposed();
            EnsureNotEmpty();
            if ((startIndex < 0) || (startIndex >= Length))
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

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="SafeString" /> instance is disposed</exception>
        public void Clear()
        {
            EnsureNotDisposed();
            for (var i = 0; i < Length; i++)
                Remove(i);
            _charBytesList.Clear();
        }

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="SafeString" /> instance is disposed</exception>
        /// <exception cref="InvalidOperationException">Throws if the <see cref="SafeString" /> instance is empty.</exception>
        public ISafeBytes ToSafeBytes()
        {
            EnsureNotDisposed();
            EnsureNotEmpty();
            var safeBytes = _safeBytesFactory.Create();
            foreach (var charBytes in _charBytesList)
                safeBytes.Append(charBytes.DeepClone());
            return safeBytes;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                const int multiplier = 27;
                var hashCode = 1;
                for (var i = 0; i < Length; i++)
                    hashCode *= multiplier*_charBytesList[i].GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(string other)
        {
            if (string.IsNullOrEmpty(other))
                return this.Length == 0;
            using (var ss = _safeStringFactory.Create())
            {
                ss.Append(other);
                return Equals(ss);
            }
        }

        public bool Equals(ISafeString other)
        {
            // Caution: Don't check length first and then fall out, since that leaks length info
            if (IsNullOrEmpty(other))
                return this.Length == 0;
            var comparisonBit = (uint)this.Length ^ (uint)other.Length;
            for (var i = 0; i < Length && i < other.Length; i++)
                comparisonBit |= (uint)(GetAsSafeBytes(i).Equals(other.GetAsSafeBytes(i)) ? 0 : 1);
            return comparisonBit == 0;
        }

        /// <inheritdoc />
        /// <exception cref="T:System.ObjectDisposedException">Throws if the <see cref="T:SafeOrbit.Memory.SafeString" /> instance is disposed</exception>
        public ISafeString DeepClone()
        {
            EnsureNotDisposed();
            var result = _safeStringFactory.Create();
            for (var i = 0; i < Length; i++)
                result.Append(GetAsSafeBytes(i).DeepClone(), InnerEncoding);
            return result;
        }

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="SafeString" /> instance is disposed</exception>
        public ISafeString ShallowClone()
        {
            EnsureNotDisposed();
            // The MemberwiseClone method creates a shallow copy by creating a new object,
            // and then copying the nonstatic fields of the current object to the new object.
            // If a field is a value type, a bit - by - bit copy of the field is performed.
            // If a field is a reference type, the reference is copied but the referred object is not;
            // therefore, the original object and its clone refer to the same object.
            // Documentation : https://msdn.microsoft.com/en-us/library/system.object.memberwiseclone.aspx
            return MemberwiseClone() as ISafeString;
        }

        public void Dispose()
        {
            Clear();
            IsDisposed = true;
        }

        public static bool IsNullOrEmpty(ISafeString safeString)
            => (safeString == null) || safeString.IsDisposed || safeString.IsEmpty;

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case ISafeString safeString:
                    return Equals(safeString);
                case string @string:
                    return Equals(@string);
                default:
                    return false;
            }
        }

        private char TransformSafeBytesToChar(ISafeBytes safeBytes, Encoding encoding)
        {
            var byteBuffer = safeBytes.ToByteArray();
            try
            {
                return _textService.GetChars(byteBuffer, InnerEncoding).First();
            }
            finally
            {
                Array.Clear(byteBuffer, 0, byteBuffer.Length);
            }
        }

        private ISafeBytes TransformCharToSafeBytes(char c, Encoding encoding)
        {
            byte[] byteBuffer = null;
            try
            {
                byteBuffer = _textService.GetBytes(c, encoding);
                var charBytes = _safeBytesFactory.Create();
                for (var i = 0; i < byteBuffer.Length; i++)
                {
                    charBytes.Append(byteBuffer[i]);
                    byteBuffer[i] = 0;
                }
                return charBytes;
            }
            finally
            {
                if(byteBuffer != null)
                    Array.Clear(byteBuffer, 0, byteBuffer.Length);
            }
        }

        /// <exception cref="ObjectDisposedException">Throws if the <see cref="SafeString" /> instance is disposed.</exception>
        private void EnsureNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(SafeString));
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
            var asString = "";
            for (var i = 0; i < Length; i++)
                asString += GetAsChar(i);
            return asString;
        }
#endif
    }
}