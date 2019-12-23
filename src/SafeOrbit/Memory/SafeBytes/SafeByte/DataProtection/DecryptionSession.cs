using System;
using SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    public class DecryptionSession : IDecryptionSession
    {
        private readonly IByteArrayProtector _protector;
        public DecryptionSession(IByteArrayProtector protector, ref byte[] encryptedBytes)
        {
            _protector = protector;
            PlainBytes = encryptedBytes;
            protector.Protect(PlainBytes);
        }
        public void Dispose() => _protector.Protect(PlainBytes);
        public byte[] PlainBytes { get; }
    }
}