using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Extensions;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices;
using SafeOrbit.Memory.SafeBytesServices.Collection;
using SafeOrbit.Memory.SafeBytesServices.Factory;

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
        /// <inheritdoc cref="Append(ISafeByte)"/>
        public void Append(byte b)
        {
            var safeByte = _safeByteFactory.GetByByte(b);
            Append(safeByte);
        }

        /// <inheritdoc />
        /// <inheritdoc cref="Append(IEnumerable{ISafeByte})"/>
        public void AppendMany(SafeMemoryStream stream)
        {
            var safeBytes = _safeByteFactory.GetByBytes(stream);
            Append(safeBytes);
        }

        /// <inheritdoc />
        /// <inheritdoc cref="Append(ISafeByte)"/>
        /// <inheritdoc cref="EnsureNotDisposed"/>
        /// <exception cref="ArgumentException">Throws if the argument is empty</exception>
        public void Append(ISafeBytes safeBytes)
        {
            EnsureNotDisposed();
            if (safeBytes.Length == 0) throw new ArgumentException($"{nameof(safeBytes)} is empty.");
            //If it's the known SafeBytes then it reveals nothing in the memory
            if (safeBytes is SafeBytes asSafeBytes)
                for (var i = 0; i < safeBytes.Length; i++)
                {
                    var safeByte = TaskContext.RunSync(() => asSafeBytes.GetAsSafeByteAsync(i));
                    Append(safeByte);
                }
            //If it's not, then reveals each byte in memory.
            else
                for (var i = 0; i < safeBytes.Length; i++)
                {
                    var safeByte = _safeByteFactory.GetByByte(safeBytes.GetByte(i));
                    Append(safeByte);
                }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Throws if the position is lower than 0 or greater than/equals to the
        ///     length
        /// </exception>
        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        /// <exception cref="InvalidOperationException">Throws if the SafeBytes instance is empty.</exception>
        public byte GetByte(int position)
        {
            EnsureNotDisposed();
            EnsureNotEmpty();
            if (position < 0 && position >= Length) throw new ArgumentOutOfRangeException(nameof(position));
            var safeByte = TaskContext.RunSync(() => GetAsSafeByteAsync(position));
            return safeByte.Get();
        }

        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        /// <exception cref="InvalidOperationException">Throws if the SafeBytes instance is empty.</exception>
        public byte[] ToByteArray()
        {
            EnsureNotDisposed();
            if (Length == 0)
                return new byte[0];
            var decryptedBytes = _safeByteCollection.ToDecryptedBytes();
            return decryptedBytes;
        }

        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        public ISafeBytes DeepClone()
        {
            EnsureNotDisposed();
            var clone = _safeBytesFactory.Create();
            if (clone is SafeBytes asSafeBytes)
            {
                //If it's the known SafeBytes then it reveals nothing in the memory
                for (var i = 0; i < Length; i++)
                    asSafeBytes.Append(TaskContext.RunSync(() => GetAsSafeByteAsync(i)));
                return asSafeBytes;
            }

            //If it's not, then reveals each byte in memory.
            for (var i = 0; i < Length; i++)
                clone.Append(GetByte(i));
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

        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        /// <exception cref="ArgumentNullException"><paramref name="safeByte" /> is <see langword="null" />.</exception>
        private void Append(ISafeByte safeByte)
        {
            EnsureNotDisposed();
            if (safeByte == null) throw new ArgumentNullException(nameof(safeByte));
            _safeByteCollection.Append(safeByte);
        }

        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        /// <exception cref="ArgumentNullException"><paramref name="safeBytes" /> is <see langword="null" />.</exception>
        private void Append(IEnumerable<ISafeByte> safeBytes)
        {
            EnsureNotDisposed();
            if (safeBytes == null) throw new ArgumentNullException(nameof(safeBytes));
            _safeByteCollection.AppendMany(safeBytes);
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

        public bool Equals(byte[] other)
        {
            if (other == null || !other.Any()) return Length == 0;
            // CAUTION: Don't check length first and then fall out, since that leaks length info during timing attacks
            var comparisonBit = (uint) Length ^ (uint) other.Length;
            // TODO: Can run in parallel
            for (var i = 0; i < Length && i < other.Length; i++)
            {
                var existingByte = TaskContext.RunSync(() => GetAsSafeByteAsync(i));
                var result = existingByte.Equals(other[i]);
                comparisonBit |= (uint) (result ? 0 : 1);
            }

            return comparisonBit == 0;
        }

        public bool Equals(ISafeBytes other)
        {
            if (other == null || other.Length == 0) return Length == 0;
            // CAUTION: Don't check length first and then fall out, since that leaks length info during timing attacks
            var comparisonBit = (uint) Length ^ (uint) other.Length;
            // TODO: Can run in parallel
            for (var i = 0; i < Length && i < other.Length; i++)
            {
                var existingByte = TaskContext.RunSync(() => GetAsSafeByteAsync(i));
                bool bytesEqual;
                if (other is SafeBytes safeBytes)
                {
                    // No need to reveal the bytes
                    var otherByte = TaskContext.RunSync(() => safeBytes.GetAsSafeByteAsync(i));
                    bytesEqual = existingByte.Equals(otherByte);
                }
                else
                {
                    bytesEqual = existingByte.Equals(other.GetByte(i)); // reveal the byte;
                }

                comparisonBit |= (uint) (bytesEqual ? 0 : 1);
            }

            return comparisonBit == 0;
        }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                SafeBytes sb => Equals(sb),
                byte[] @byte => Equals(@byte),
                _ => false
            };
        }

        public override int GetHashCode()
        {
            unchecked
            {
                //TODO: Better parallel implementation
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