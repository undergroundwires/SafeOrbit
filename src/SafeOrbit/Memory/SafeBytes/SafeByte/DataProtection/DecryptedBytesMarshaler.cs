using System;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    public class DecryptedBytesMarshaler : IDecryptedBytesMarshaler
    {
        private readonly byte[] _plainBytes;
        private volatile bool _isDisposed;

        /// <exception cref="ArgumentException">Throws if the <paramref name="decryptedBytes" /> is empty.</exception>
        /// <exception cref="ArgumentNullException">Throws if the <paramref name="decryptedBytes" /> is null.</exception>
        public DecryptedBytesMarshaler(byte[] decryptedBytes)
        {
            if (decryptedBytes == null) throw new ArgumentNullException(nameof(decryptedBytes));
            if (decryptedBytes.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(decryptedBytes));
            _plainBytes = decryptedBytes;
        }

        /// <exception cref="ObjectDisposedException" accessor="get">If object is disposed</exception>
        public byte[] PlainBytes
        {
            get
            {
                EnsureNotDisposed();
                return _plainBytes;
            }
        }

        /// <exception cref="ObjectDisposedException">If object is already disposed</exception>
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