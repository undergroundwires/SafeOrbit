using System;
using System.Security.Cryptography;
using SafeOrbit.Extensions;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    public class MemoryProtectedBytes : IMemoryProtectedBytes
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

        /// <exception cref="ObjectDisposedException">Throws if the instance is already disposed</exception>
        public void Dispose()
        {
            EnsureNotDisposed();
            IsDisposed = true;
            if (_encryptedBytes != null)
                Array.Clear(_encryptedBytes, 0, _encryptedBytes.Length);
        }

        /// <exception cref="ObjectDisposedException">Throws if the instance is already disposed</exception>
        /// <exception cref="InvalidOperationException">Throws if the instance is already initialized.</exception>
        /// <exception cref="ArgumentException">Throws if the <paramref name="plainBytes" /> is empty.</exception>
        /// <exception cref="ArgumentNullException">Throws if the <paramref name="plainBytes" /> is null.</exception>
        /// <exception cref="CryptographicException">
        ///     Throws if the <paramref name="plainBytes" /> size does not conform to block
        ///     size defined in <see cref="BlockSizeInBytes" />.
        /// </exception>
        public void Initialize(byte[] plainBytes)
        {
            if (plainBytes == null) throw new ArgumentNullException(nameof(plainBytes));
            if (plainBytes.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(plainBytes));
            if (plainBytes.Length % BlockSizeInBytes != 0)
                throw new CryptographicException(
                    $"Block size is {BlockSizeInBytes}, but plain bytes have {plainBytes.Length}. Maybe pad the bytes?");
            if (IsInitialized) throw new InvalidOperationException("Already initialized");
            EnsureNotDisposed();
            _encryptedBytes = plainBytes;
            _protector.Protect(_encryptedBytes);
        }

        public int BlockSizeInBytes => _protector.BlockSizeInBytes;
        public bool IsInitialized => _encryptedBytes != null;
        public bool IsDisposed { get; private set; }

        /// <exception cref="ObjectDisposedException">Throws if the instance is already disposed</exception>
        /// <exception cref="InvalidOperationException">Throws if the instance is not yet initialized.</exception>
        public IDecryptedBytesMarshaler RevealDecryptedBytes()
        {
            if (!IsInitialized) throw new InvalidOperationException("Not yet initialized");
            EnsureNotDisposed();
            var decryptedBytes = _encryptedBytes.CopyToNewArray();
            _protector.Unprotect(decryptedBytes);
            return new DecryptedBytesMarshaler(decryptedBytes);
        }

        /// <exception cref="ObjectDisposedException">Throws if the instance is already disposed</exception>
        public IMemoryProtectedBytes DeepClone()
        {
            EnsureNotDisposed();
            var bytes = IsInitialized ? _encryptedBytes.CopyToNewArray() : null;
            return new MemoryProtectedBytes(_protector, bytes);
        }

        private void EnsureNotDisposed()
        {
            if (IsDisposed) throw new ObjectDisposedException(GetType().Name);
        }
    }
}