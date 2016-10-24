using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using SafeOrbit.Random.Tinhat;

namespace SafeOrbit.Random
{
    public interface IEntropyHasher : IDisposable
    {
        RandomNumberGenerator Rng { get; }
        IList<HashAlgorithmWrapper> HashWrappers { get; }
    }
}