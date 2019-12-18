using System;
using SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    public class DecryptionSession : IDecryptionSession
    {
        private readonly IByteArrayProtector _protector;
        private readonly byte[] _decryptedBytes;
        public DecryptionSession(IByteArrayProtector protector, ref byte[] encryptedBytes)
        {
            _protector = protector;
            _decryptedBytes = encryptedBytes;
            protector.Protect(_decryptedBytes);
        }
        public void Dispose() => _protector.Protect(_decryptedBytes);
        public byte[] PlainBytes => _decryptedBytes;
    }
}