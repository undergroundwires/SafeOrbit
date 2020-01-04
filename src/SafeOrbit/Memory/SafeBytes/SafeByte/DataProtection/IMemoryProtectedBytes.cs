using System;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    public interface IMemoryProtectedBytes : IDisposable, IDeepCloneable<IMemoryProtectedBytes>
    {
        int BlockSizeInBytes { get; }
        bool IsInitialized { get; }
        bool IsDisposed { get; }
        void Initialize(byte[] plainBytes);
        IDecryptedBytesMarshaler RevealDecryptedBytes();
    }
}