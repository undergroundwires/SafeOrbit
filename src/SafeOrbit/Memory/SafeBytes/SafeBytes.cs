using System;
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
        private const int TotalArbitraryBytesPerByte = 1;
        private readonly IFastRandom _fastRandom;
        private readonly ISafeByteCollection _safeByteCollection;
        private readonly ISafeByteFactory _safeByteFactory;
        private readonly IFactory<ISafeBytes> _safeBytesFactory;

        [DebuggerHidden]
        public SafeBytes() : this(
            SafeOrbitCore.Current.Factory.Get<IFastRandom>(),
            SafeOrbitCore.Current.Factory.Get<ISafeByteFactory>(),
            SafeOrbitCore.Current.Factory.Get<IFactory<ISafeBytes>>(),
            SafeOrbitCore.Current.Factory.Get<IFactory<ISafeByteCollection>>())
        {
        }

        internal SafeBytes(
            IFastRandom fastRandom,
            ISafeByteFactory safeByteFactory,
            IFactory<ISafeBytes> safeBytesFactory,
            IFactory<ISafeByteCollection> safeByteCollectionFactory)
        {
            _fastRandom = fastRandom ?? throw new ArgumentNullException(nameof(fastRandom));
            _safeByteFactory = safeByteFactory ?? throw new ArgumentNullException(nameof(safeByteFactory));
            _safeBytesFactory = safeBytesFactory ?? throw new ArgumentNullException(nameof(safeBytesFactory));
            _safeByteCollection = safeByteCollectionFactory.Create();
        }

        /// <summary>
        ///     Returns to real length of the bytes inside
        /// </summary>
        public int Length => (_safeByteCollection?.Length ?? 0) == 0
            ? 0
            : _safeByteCollection.Length / (TotalArbitraryBytesPerByte + 1);

        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Adds the byte, and one random byte after it to avoid memory leaking
        /// </summary>
        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        public void Append(byte b)
        {
            var safeByte = _safeByteFactory.GetByByte(b);
            Append(safeByte);
        }

        /// <summary>
        ///     Clones and appends the each byte from given SafeBytes object to the end.
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws if the argument is null</exception>
        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
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

        /// <summary>
        ///     Gets the byte in the safe list
        /// </summary>
        /// <param name="position">Index of the byte</param>
        /// <returns>Byte from the array</returns>
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
            var withoutArbitraryBytes = decryptedBytes.Where((x, i) => i % (TotalArbitraryBytesPerByte + 1) == 0)
                .ToArray();
            return withoutArbitraryBytes;
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

        /// <exception cref="ObjectDisposedException">Throws if the SafeBytes instance is disposed</exception>
        /// <exception cref="ArgumentNullException"><paramref name="safeByte" /> is <see langword="null" />.</exception>
        private void Append(ISafeByte safeByte)
        {
            EnsureNotDisposed();
            if (safeByte == null) throw new ArgumentNullException(nameof(safeByte));
            _safeByteCollection.Append(safeByte);
            AddArbitraryByte();
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
            var realPosition = position * (TotalArbitraryBytesPerByte + 1);
            if (realPosition < 0 || realPosition >= _safeByteCollection.Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            return await _safeByteCollection.GetAsync(realPosition).ConfigureAwait(false);
        }

        /// <summary>
        ///     The reason to add arbitrary byte is to avoid simple memory dumps to see all bytes.
        /// </summary>
        private void AddArbitraryByte()
        {
            for (var i = 0; i < TotalArbitraryBytesPerByte; i++)
            {
                var safeByte = _safeByteFactory.GetByByte((byte) _fastRandom.GetInt(0, 256));
                _safeByteCollection.Append(safeByte);
            }
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