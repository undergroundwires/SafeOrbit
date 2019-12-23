using System;
using System.Runtime.CompilerServices;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Library;
using SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    public class MemoryProtectedBytes : IMemoryProtectedBytes
    {
        private bool _isDisposed = false;
        private readonly IByteArrayProtector _protector;
        private byte[] _encryptedBytes;
        public MemoryProtectedBytes(): this(SafeOrbitCore.Current.Factory.Get<IByteArrayProtector>())
        {
        }
        internal MemoryProtectedBytes(IByteArrayProtector protector)
        {
            _protector = protector ?? throw new ArgumentNullException(nameof(protector));
        }
        private MemoryProtectedBytes(IByteArrayProtector protector, byte[] encryptedBytes)
            :this(protector)
        {
            _encryptedBytes = encryptedBytes;
        }

        public void Dispose()
        {
            if (_isDisposed)
                throw new ArgumentException("Already disposed");
            _isDisposed = true;
            if(_encryptedBytes != null)
                Array.Clear(_encryptedBytes, 0, _encryptedBytes.Length);
        }
        public void Initialize(byte[] plainBytes)
        {
            if (plainBytes == null)  throw new ArgumentNullException(nameof(plainBytes));
            if (plainBytes.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(plainBytes));
            if (IsInitialized)  throw new ArgumentException("Already initialized");
            if(_isDisposed) throw new ArgumentException("Dispose instance cannot be initialized");
            if(plainBytes.Length % BlockSizeInBytes != 0) throw new ArgumentException($"Block size is {BlockSizeInBytes}, but plain bytes have {plainBytes.Length}. Maybe pad the bytes?");
            _encryptedBytes = plainBytes;
            _protector.Protect(_encryptedBytes);
        }
        public int BlockSizeInBytes => _protector.BlockSizeInBytes;
        public bool IsInitialized => _encryptedBytes != null;

        public bool IsDisposed => _isDisposed;

        //TODO: If we copy the encrypted bytes here instead of sending a reference we achieve the multi thread safety very easily
        public IDecryptionSession RevealDecryptedBytes()
        {
            if (!IsInitialized)  throw new ArgumentException("Not yet initialized");
            if(IsDisposed)  throw new ArgumentException("Instance is disposed");
            return new DecryptionSession(_protector, ref _encryptedBytes);
        }

        public IMemoryProtectedBytes DeepClone()
        { 
            if (IsDisposed) throw new ArgumentException("Cannot clone a disposed instance");
            var bytes = this.IsInitialized ? GetCopyOfBytes(this._encryptedBytes) : null; 
            return new MemoryProtectedBytes(_protector, bytes);
        }

        private static byte[] GetCopyOfBytes(byte[] source)
        {
            var newEncryptedBytes = new byte[source.Length];
            Array.Copy(source, newEncryptedBytes, source.Length);
            return newEncryptedBytes;
        }
    }
}
