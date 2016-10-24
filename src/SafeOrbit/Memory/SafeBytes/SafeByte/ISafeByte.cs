namespace SafeOrbit.Memory.SafeBytesServices
{
    using Interfaces;
    using System;

    internal interface ISafeByte : IDisposable,
        IEquatable<ISafeByte>, IEquatable<byte>,
        IDeepCloneable<ISafeByte>
    {
        int Id { get; }
        bool IsByteSet { get; }
        void Set(byte b);
        byte Get();
    }
}