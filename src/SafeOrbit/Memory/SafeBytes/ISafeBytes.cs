using System;

namespace SafeOrbit.Memory
{
    public interface ISafeBytes : IDisposable, IDeepCloneable<ISafeBytes>,
        IEquatable<byte[]>, IEquatable<ISafeBytes>
    {
        int Length { get; }
        bool IsDisposed { get; }
        void Append(byte b);
        void Append(ISafeBytes safeBytes);
        byte GetByte(int position);
        byte[] ToByteArray();
        int GetHashCode();
    }
}