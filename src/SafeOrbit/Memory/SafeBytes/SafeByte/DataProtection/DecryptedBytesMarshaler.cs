using System;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    public class DecryptedBytesMarshaler : IDecryptedBytesMarshaler
    {
        private volatile bool _isDisposed = false;
        private readonly byte[] _plainBytes;

        public byte[] PlainBytes
        {
            get
            {
                EnsureNotDisposed();
                return _plainBytes;
            }
        }

        public DecryptedBytesMarshaler(byte[] decryptedBytes)
        {
            if (decryptedBytes == null) throw new ArgumentNullException(nameof(decryptedBytes));
            if (decryptedBytes.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(decryptedBytes));
            this._plainBytes = decryptedBytes;
        }
        public void Dispose()
        {
            EnsureNotDisposed();
            Array.Clear(PlainBytes, 0, PlainBytes.Length);
            _isDisposed = true;
        }
        private void EnsureNotDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(DecryptedBytesMarshaler));
        }
    }
}