using System;

namespace SafeOrbit.Memory.Injection
{
    internal interface IStamp<THash> : IEquatable<IStamp<THash>>
    {
        THash Hash { get; }
    }
    internal interface IStamp : IStamp<int>{ }
}