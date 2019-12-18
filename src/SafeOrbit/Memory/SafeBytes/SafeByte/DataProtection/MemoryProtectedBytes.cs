using System;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    public class MemoryProtectedBytes : IMemoryProtectedBytes
    {
        private readonly IByteArrayProtector _protector;
        private byte[] _encryptedBytes;
        public MemoryProtectedBytes(): this(SafeOrbitCore.Current.Factory.Get<IByteArrayProtector>())
        {
        }
        internal MemoryProtectedBytes(IByteArrayProtector protector)
        {
            _protector = protector ?? throw new ArgumentNullException(nameof(protector));
        }
        public void Dispose() => Array.Clear(_encryptedBytes, 0, _encryptedBytes.Length);
        public void Initialize(byte[] plainBytes)
        {
            if (plainBytes?.Length == 0)
                throw new ArgumentException("Value cannot be a null or empty collection.", nameof(plainBytes));
            if (IsInitialized)
                throw new ArgumentException("Already initialized");
            _encryptedBytes = plainBytes;
            _protector.Protect(_encryptedBytes);
        }

        public int BlockSizeInBytes => _protector.BlockSizeInBytes;
        public bool IsInitialized => _encryptedBytes != null;
        public IDecryptionSession RevealDecryptedBytes() => new DecryptionSession(
            _protector, ref _encryptedBytes);

        public IMemoryProtectedBytes DeepClone()
        {
            throw new NotImplementedException();
        }
    }
}
