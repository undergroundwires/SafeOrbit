using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SafeOrbit.Common;
using SafeOrbit.Extensions;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices;
using SafeOrbit.Memory.SafeBytesServices.Collection;
using SafeOrbit.Memory.SafeBytesServices.Factory;

namespace SafeOrbit.Memory
{
    /// <remarks>
    ///     Wraps <see cref="ISafeByteCollection" /> and extends it with more functionality.
    /// </remarks>
    public class SafeBytes : DisposableBase, ISafeBytes
    {
        private int _hashcode = 0x2001D81A;

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
        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        /// <exception cref="ArgumentException">Throws if the argument is empty</exception>
        public async Task AppendAsync(ISafeBytes safeBytes)
        {
            ThrowIfDisposed();
            if (safeBytes.Length == 0) throw new ArgumentException($"{nameof(safeBytes)} is empty.");
            // If it's the known SafeBytes then it reveals nothing in the memory
            if (safeBytes is SafeBytes asSafeBytes)
            {
                var allBytes = await asSafeBytes._safeByteCollection.GetAllAsync()
                    .ConfigureAwait(false);
                await _safeByteCollection.AppendManyAsync(allBytes)
                    .ConfigureAwait(false);
                foreach (var safeByte in allBytes)
                    UpdateHashCode(safeByte);
            }
            // If it's not, then reveals each byte in memory.
            else
            {
                var plainBytes = await safeBytes.ToByteArrayAsync()
                    .ConfigureAwait(false);
                var stream = new SafeMemoryStream(plainBytes);
                await AppendManyAsync(stream).ConfigureAwait(false);
            }
        }
        /// <inheritdoc />
        /// <inheritdoc cref="AppendManyAsync(ISafeByte[])"/>
        public async Task AppendManyAsync(SafeMemoryStream stream)
        {
            var safeBytes = await _safeByteFactory.GetByBytesAsync(stream)
                .ConfigureAwait(false);
            await AppendManyAsync(safeBytes as ISafeByte[] ?? safeBytes.ToArray())
                .ConfigureAwait(false);
        }


        /// <inheritdoc />
        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Throws if the position is lower than 0 or greater than/equals to the
        ///     length
        /// </exception>
        /// <exception cref="InvalidOperationException">Throws if the SafeBytes instance is empty.</exception>
        public async Task<byte> GetByteAsync(int position)
        {
            ThrowIfDisposed();
            EnsureNotEmpty();
            if (position < 0 && position >= Length) throw new ArgumentOutOfRangeException(nameof(position));
            var safeByte = await GetAsSafeByteAsync(position)
                .ConfigureAwait(false);
            var @byte = await safeByte.GetAsync()
                .ConfigureAwait(false);
            return @byte;
        }


        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        /// <exception cref="InvalidOperationException">Throws if the SafeBytes instance is empty.</exception>
        public async Task<byte[]> ToByteArrayAsync()
        {
            ThrowIfDisposed();
            if (Length == 0)
                return new byte[0];
            var decryptedBytes = await _safeByteCollection.ToDecryptedBytesAsync()
                .ConfigureAwait(false);
            return decryptedBytes;
        }

        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        public async Task<ISafeBytes> DeepCloneAsync()
        {
            ThrowIfDisposed();
            var clone = _safeBytesFactory.Create();
            await clone.AppendAsync(this).ConfigureAwait(false);
            return clone;
        }
        /// <summary>
        ///     Indicates whether the specified SafeBytes object is null or holds zero bytes.
        /// </summary>
        public static bool IsNullOrEmpty(ISafeBytes safeBytes) => safeBytes == null || safeBytes.Length == 0;


        /// <summary>
        ///     Gets the byte from its real location 
        /// </summary>
        internal async Task<ISafeByte> GetAsSafeByteAsync(int position)
        {
            if (position < 0 || position >= _safeByteCollection.Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            return await _safeByteCollection.GetAsync(position)
                .ConfigureAwait(false);
        }

        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        /// <exception cref="ArgumentNullException"><paramref name="safeByte" /> is <see langword="null" />.</exception>
        private async Task AppendAsync(ISafeByte safeByte)
        {
            ThrowIfDisposed();
            if (safeByte == null) throw new ArgumentNullException(nameof(safeByte));
            await _safeByteCollection.AppendAsync(safeByte)
                .ConfigureAwait(false);
            UpdateHashCode(safeByte);
        }

        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        /// <exception cref="ArgumentNullException"><paramref name="safeBytes" /> is <see langword="null" />.</exception>
        private async Task AppendManyAsync(ISafeByte[] safeBytes)
        {
            ThrowIfDisposed();
            if (safeBytes == null) throw new ArgumentNullException(nameof(safeBytes));
            await _safeByteCollection.AppendManyAsync(safeBytes)
                .ConfigureAwait(false);
            foreach (var @byte in safeBytes)
                UpdateHashCode(@byte);
        }

        /// <exception cref="InvalidOperationException">Throws if the <see cref="SafeBytes"/> instance is empty.</exception>
        private void EnsureNotEmpty()
        {
            if (Length == 0) throw new InvalidOperationException($"{nameof(SafeBytes)} is empty");
        }

        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        public async Task<bool> EqualsAsync(byte[] other)
        {
            ThrowIfDisposed();
            if (other == null || !other.Any()) return Length == 0;

            var otherBytes = GetOtherBytes();
            var ownBytes = _safeByteCollection.GetAllAsync();
            await Task.WhenAll(otherBytes, ownBytes)
                .ConfigureAwait(false);

            return AreEqual(otherBytes.Result, ownBytes.Result);
            
            async Task<ISafeByte[]> GetOtherBytes()
            {
                var stream = new SafeMemoryStream(other.CopyToNewArray());
                var bytes = await _safeByteFactory.GetByBytesAsync(stream).ConfigureAwait(false);
                return bytes.ToArray();
            }
        }

        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        public async Task<bool> EqualsAsync(ISafeBytes other)
        {
            ThrowIfDisposed();
            if (other == null || other.Length == 0) return Length == 0;
            if (this.GetHashCode() != other.GetHashCode())
                return false;

            var otherBytes = GetOtherBytesAsync();
            var ownBytes = _safeByteCollection.GetAllAsync();
            await Task.WhenAll(otherBytes, ownBytes)
                .ConfigureAwait(false);

            return AreEqual(otherBytes.Result, ownBytes.Result);

            async Task<ISafeByte[]> GetOtherBytesAsync()
            {
                if (other is SafeBytes safeBytes)
                {
                    // If it's the known SafeBytes then it reveals nothing in the memory
                    var bytes = await safeBytes._safeByteCollection.GetAllAsync()
                        .ConfigureAwait(false);
                    return bytes;
                }
                else
                {
                    // If it's not, then reveals each byte in memory.
                    var bytes = await other.ToByteArrayAsync().ConfigureAwait(false);
                    var stream = new SafeMemoryStream(bytes);
                    var safe = await _safeByteFactory.GetByBytesAsync(stream)
                        .ConfigureAwait(false);
                    return safe.ToArray();
                }
            }
        }

        /// <exception cref="NotSupportedException">Use <see cref="EqualsAsync(byte[])"/> or <see cref="EqualsAsync(ISafeBytes)"/> instead</exception>
        public override bool Equals(object obj)
        {
            throw new NotSupportedException($"Use {nameof(EqualsAsync)} instead");
        }

        public override int GetHashCode() => _hashcode;

        protected override void DisposeManagedResources()
        {
            _safeByteCollection?.Dispose();
        }

        private static bool AreEqual(IReadOnlyList<ISafeByte> first, IReadOnlyList<ISafeByte> second)
        {
            var comparisonBit = (uint)first.Count ^ (uint)second.Count; // Timing attack caution: Don't check length first and then fall out (it leaks length info)
            for (var i = 0; i < first.Count && i < second.Count; i++)
            {
                var result = first[i].Equals(second[i]);
                comparisonBit |= (uint)(result ? 0 : 1); // Timing attack caution: Do not fallout directly
            }
            return comparisonBit == 0;
        }

        private void UpdateHashCode(ISafeByte newByte)
        {
            unchecked
            {
                _hashcode *= 31 + newByte.Id;
            }
        }
    }
}