using System;
using System.Threading.Tasks;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    public interface IMemoryProtectedBytes : IDisposable, IDeepCloneable<IMemoryProtectedBytes>
    {
        int BlockSizeInBytes { get; }
        bool IsInitialized { get; }
        bool IsDisposed { get; }
        Task InitializeAsync(byte[] plainBytes);
        Task<IDecryptedBytesMarshaler> RevealDecryptedBytesAsync();
    }
}