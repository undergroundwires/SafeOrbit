using System;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    public interface IDecryptedBytesMarshaler : IDisposable
    {
        byte[] PlainBytes { get; }
    }
}