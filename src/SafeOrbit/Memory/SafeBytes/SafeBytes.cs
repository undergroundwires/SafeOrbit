using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices;
using SafeOrbit.Memory.SafeBytesServices.Collection;
using SafeOrbit.Memory.SafeBytesServices.Factory;
using SafeOrbit.Parallel;

namespace SafeOrbit.Memory
{
    /// <remarks>
    ///     Wraps <see cref="ISafeByteCollection" />.
    ///     Adds arbitrary bytes during modifications to avoid having real sequences in the memory.
    /// </remarks>
    public class SafeBytes : ISafeBytes
    {
        private readonly ISafeByteCollection _safeByteCollection;
        private readonly ISafeByteFactory _safeByteFactory;
        private readonly IFactory<ISafeBytes> _safeBytesFactory;

        [DebuggerHidden]
        public SafeBytes() : this(
            SafeOrbitCore.Current.Factory.Get<ISafeByteFactory>(),
            SafeOrbitCore.Current.Factory.Get<IFactory<ISafeBytes>>(),
            SafeOrbitCore.Current.Factory.Get<IFactory<ISafeByteCollection>>())
        {
        }

        internal SafeBytes(
            ISafeByteFactory safeByteFactory,
            IFactory<ISafeBytes> safeBytesFactory,
            IFactory<ISafeByteCollection> safeByteCollectionFactory)
        {
            _safeByteFactory = safeByteFactory ?? throw new ArgumentNullException(nameof(safeByteFactory));
            _safeBytesFactory = safeBytesFactory ?? throw new ArgumentNullException(nameof(safeBytesFactory));
            _safeByteCollection = safeByteCollectionFactory.Create();
        }

        /// <inheritdoc />
        public int Length => _safeByteCollection?.Length ?? 0;

        public bool IsDisposed { get; private set; }

        /// <inheritdoc />
        /// <inheritdoc cref="AppendAsync(ISafeByte)"/>
        public async Task AppendAsync(byte @byte)
        {
            var safeByte = await _safeByteFactory.GetByByteAsync(@byte)
                .ConfigureAwait(false);
            await AppendAsync(safeByte)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        /// <inheritdoc cref="AppendManyAsync"/>
        public async Task AppendManyAsync(SafeMemoryStream stream)
        {
            var safeBytes = await _safeByteFactory.GetByBytesAsync(stream)
                .ConfigureAwait(false);
            await AppendManyAsync(safeBytes)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        /// <inheritdoc cref="AppendAsync(ISafeByte)"/>
        /// <inheritdoc cref="EnsureNotDisposed"/>
        /// <exception cref="ArgumentException">Throws if the argument is empty</exception>
        public async Task AppendAsync(ISafeBytes safeBytes)
        {
            EnsureNotDisposed();
            if (safeBytes.Length == 0) throw new ArgumentException($"{nameof(safeBytes)} is empty.");
            //If it's the known SafeBytes then it reveals nothing in the memory
            if (safeBytes is SafeBytes asSafeBytes)
                for (var i = 0; i < safeBytes.Length; i++)
                {
                    var safeByte = await asSafeBytes.GetAsSafeByteAsync(i)
                        .ConfigureAwait(false);
                    await AppendAsync(safeByte).ConfigureAwait(false);
                }
            //If it's not, then reveals each byte in memory.
            else
                for (var i = 0; i < safeBytes.Length; i++)
                {
                    var @byte = await safeBytes.GetByteAsync(i).ConfigureAwait(false);
                    var safeByte = await _safeByteFactory.GetByByteAsync(@byte)
                        .ConfigureAwait(false);
                    await AppendAsync(safeByte).ConfigureAwait(false);
                }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Throws if the position is lower than 0 or greater than/equals to the
        ///     length
        /// </exception>
        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        /// <exception cref="InvalidOperationException">Throws if the SafeBytes instance is empty.</exception>
        public async Task<byte> GetByteAsync(int position)
        {
            EnsureNotDisposed();
            EnsureNotEmpty();
            if (position < 0 && position >= Length) throw new ArgumentOutOfRangeException(nameof(position));
            var safeByte = await GetAsSafeByteAsync(position)
                .ConfigureAwait(false);
            var @byte = await safeByte.GetAsync()
                .ConfigureAwait(false);
            return @byte;
        }

        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        /// <exception cref="InvalidOperationException">Throws if the SafeBytes instance is empty.</exception>
        public async Task<byte[]> ToByteArrayAsync()
        {
            EnsureNotDisposed();
            if (Length == 0)
                return new byte[0];
            var decryptedBytes = await _safeByteCollection.ToDecryptedBytesAsync()
                .ConfigureAwait(false);
            return decryptedBytes;
        }

        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        public async Task<ISafeBytes> DeepCloneAsync()
        {
            EnsureNotDisposed();
            var clone = _safeBytesFactory.Create();
            if (clone is SafeBytes asSafeBytes)
            {
                //If it's the known SafeBytes then it reveals nothing in the memory
                for (var i = 0; i < Length; i++)
                {
                    var safeByte = await GetAsSafeByteAsync(i).ConfigureAwait(false);
                    await asSafeBytes.AppendAsync(safeByte)
                        .ConfigureAwait(false);
                }
                return asSafeBytes;
            }

            //If it's not, then reveals each byte in memory.
            for (var i = 0; i < Length; i++)
            {
                var @byte = await GetByteAsync(i).ConfigureAwait(false);
                await clone.AppendAsync(@byte).ConfigureAwait(false);
            }
            return clone;
        }
        /// <summary>
        ///     Indicates whether the specified SafeBytes object is null or holds zero bytes.
        /// </summary>
        public static bool IsNullOrEmpty(ISafeBytes safeBytes) => safeBytes == null || safeBytes.Length == 0;


        /// <summary>
        ///     Gets the byte from its real location (skips the arbitrary bytes)
        /// </summary>
        internal async Task<ISafeByte> GetAsSafeByteAsync(int position)
        {
            if (position < 0 || position >= _safeByteCollection.Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            return await _safeByteCollection.GetAsync(position)
                .ConfigureAwait(false);
        }

        /// <inheritdoc cref="EnsureNotDisposed"/>
        /// <exception cref="ArgumentNullException"><paramref name="safeByte" /> is <see langword="null" />.</exception>
        private async Task AppendAsync(ISafeByte safeByte)
        {
            EnsureNotDisposed();
            if (safeByte == null) throw new ArgumentNullException(nameof(safeByte));
            await _safeByteCollection.AppendAsync(safeByte)
                .ConfigureAwait(false);
        }

        /// <inheritdoc cref="EnsureNotDisposed"/>
        /// <exception cref="ArgumentNullException"><paramref name="safeBytes" /> is <see langword="null" />.</exception>
        private async Task AppendManyAsync(IEnumerable<ISafeByte> safeBytes)
        {
            EnsureNotDisposed();
            if (safeBytes == null) throw new ArgumentNullException(nameof(safeBytes));
            await _safeByteCollection.AppendManyAsync(safeBytes)
                .ConfigureAwait(false);
        }

        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        private void EnsureNotDisposed()
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SafeBytes));
        }

        /// <exception cref="InvalidOperationException">Throws if the SafeBytes instance is empty.</exception>
        private void EnsureNotEmpty()
        {
            if (Length == 0) throw new InvalidOperationException($"{nameof(SafeBytes)} is empty");
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed) return; //skip redundant calls
            if (disposing)
                _safeByteCollection.Dispose();
            IsDisposed = true;
        }

        ~SafeBytes()
        {
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Equality

        public async Task<bool> EqualsAsync(byte[] other)  //TODO: Utilize GetAllSafeBytes, getting one by one is so slow
        {
            if (other == null || !other.Any()) return Length == 0;
            // CAUTION: Don't check length first and then fall out, since that leaks length info during timing attacks
            var comparisonBit = (uint) Length ^ (uint) other.Length;
            // TODO: Can run in parallel
            for (var i = 0; i < Length && i < other.Length; i++)
            {
                var existingByte = await GetAsSafeByteAsync(i).ConfigureAwait(false);
                var result = await existingByte.EqualsAsync(other[i]).ConfigureAwait(false);
                comparisonBit |= (uint) (result ? 0 : 1);
            }
            return comparisonBit == 0;
        }

        public async Task<bool> EqualsAsync(ISafeBytes other) //TODO: Utilize GetAllSafeBytes, getting one by one is so slow
        {
            if (other == null || other.Length == 0) return Length == 0;
            // CAUTION: Don't check length first and then fall out, since that leaks length info during timing attacks
            var comparisonBit = (uint) Length ^ (uint) other.Length;
            // TODO: Can run in parallel
            for (var i = 0; i < Length && i < other.Length; i++)
            {
                var existingByte = await GetAsSafeByteAsync(i).ConfigureAwait(false);
                bool bytesEqual;
                if (other is SafeBytes safeBytes)
                {
                    // No need to reveal the bytes
                    var otherByte = TaskContext.RunSync(() => safeBytes.GetAsSafeByteAsync(i));
                    bytesEqual = existingByte.Equals(otherByte);
                }
                else
                {
                    var @byte = await other.GetByteAsync(i).ConfigureAwait(false);
                    bytesEqual = await existingByte.EqualsAsync(@byte).ConfigureAwait(false);
                }

                comparisonBit |= (uint) (bytesEqual ? 0 : 1);
            }

            return comparisonBit == 0;
        }

        public override bool Equals(object obj)
        {
            throw new NotSupportedException($"Use {nameof(EqualsAsync)} instead");
        }

        public override int GetHashCode() // TODO: Utilize GetAllSafeBytes, getting one by one is so slow
        {
            unchecked
            {
                const int multiplier = 26;
                var hashCode = 1;
                for (var i = 0; i < Length; i++)
                    hashCode *= multiplier * TaskContext.RunSync(() => GetAsSafeByteAsync(i)).Id;
                return hashCode;
            }
        }

        #endregion
    }
}