using System;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    public interface IDecryptionSession : IDisposable
    {
        byte[] PlainBytes { get; }
    }
}