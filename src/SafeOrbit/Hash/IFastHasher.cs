using System;

namespace SafeOrbit.Hash
{
    /// <summary>
    /// An interface for a fast and cryptological hasher
    /// </summary>
    public interface IFastHasher<out TResult, in TSeed>
    {
        TResult ComputeFast(byte[] input);
        TResult ComputeFast(byte[] input, TSeed seed);
    }

    public interface IFastHasher : IFastHasher<int, uint>
    {
        
    }
}
