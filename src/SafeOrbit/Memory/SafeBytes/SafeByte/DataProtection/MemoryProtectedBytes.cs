using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SafeOrbit.Common;
using SafeOrbit.Extensions;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    public class MemoryProtectedBytes : DisposableBase, IMemoryProtectedBytes
    {
        private readonly IByteArrayProtector _protector;
        private byte[] _encryptedBytes;

        public MemoryProtectedBytes() : this(SafeOrbitCore.Current.Factory.Get<IByteArrayProtector>())
        {
        }

        internal MemoryProtectedBytes(IByteArrayProtector protector)
        {
            _protector = protector ?? throw new ArgumentNullException(nameof(protector));
        }

        private MemoryProtectedBytes(IByteArrayProtector protector, byte[] encryptedBytes)
            : this(protector)
        {
            _encryptedBytes = encryptedBytes;
        }

        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        /// <exception cref="InvalidOperationException">Already initialized.</exception>
        /// <exception cref="ArgumentException">Throws if the <paramref name="plainBytes" /> is empty.</exception>
        /// <exception cref="ArgumentNullException">Throws if the <paramref name="plainBytes" /> is null.</exception>
        /// <exception cref="CryptographicException">
        ///     Throws if the <paramref name="plainBytes" /> size does not conform to block
        ///     size defined in <see cref="BlockSizeInBytes" />.
        /// </exception>
        public async Task InitializeAsync(byte[] plainBytes)
        {
            if (plainBytes == null) throw new ArgumentNullException(nameof(plainBytes));
            if (plainBytes.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(plainBytes));
            if (plainBytes.Length % BlockSizeInBytes != 0)
                throw new CryptographicException(
                    $"Block size is {BlockSizeInBytes}, but plain bytes have {plainBytes.Length}. Maybe pad the bytes?");
            if(IsInitialized) throw new InvalidOperationException("Already initialized");
            ThrowIfDisposed();
            var encryptedBytes = plainBytes;
            await _protector.ProtectAsync(encryptedBytes).ConfigureAwait(false);
            _encryptedBytes = encryptedBytes;
        }

        public int BlockSizeInBytes => _protector.BlockSizeInBytes;
        public bool IsInitialized => _encryptedBytes != null;
        
        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        /// <inheritdoc cref="ThrowIfNotInitialized"/>
        public async Task<IDecryptedBytesMarshaler> RevealDecryptedBytesAsync()
        {
            ThrowIfNotInitialized();
            ThrowIfDisposed();
            var decryptedBytes = _encryptedBytes.CopyToNewArray();
            await _protector.UnprotectAsync(decryptedBytes).ConfigureAwait(false);
            return new DecryptedBytesMarshaler(decryptedBytes);
        }

        /// <inheritdoc cref="DisposableBase.ThrowIfDisposed"/>
        /// <inheritdoc cref="ThrowIfNotInitialized"/>
        public IMemoryProtectedBytes DeepClone()
        {
            ThrowIfDisposed();
            var bytes = IsInitialized ? _encryptedBytes.CopyToNewArray() : null;
            return new MemoryProtectedBytes(_protector, bytes);
        }

        protected override void DisposeManagedResources()
        {
            if (_encryptedBytes != null)
                Array.Clear(_encryptedBytes, 0, _encryptedBytes.Length);
        }

        /// <exception cref="InvalidOperationException"><see cref="MemoryProtectedBytes"/> instance is not yet initialized.</exception>
        private void ThrowIfNotInitialized()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Not yet initialized");
        }
    }
}