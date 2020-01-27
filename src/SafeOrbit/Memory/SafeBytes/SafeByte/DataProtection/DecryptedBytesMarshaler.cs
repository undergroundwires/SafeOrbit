using System;
using SafeOrbit.Common;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    public class DecryptedBytesMarshaler : DisposableBase, IDecryptedBytesMarshaler
    {
        private readonly byte[] _plainBytes;

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
                ThrowIfDisposed();
                return _plainBytes;
            }
        }

        protected override void DisposeManagedResources()
        {
            if(PlainBytes != null)
                Array.Clear(PlainBytes, 0, PlainBytes.Length);
        }
    }
}