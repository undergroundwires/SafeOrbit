using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SafeOrbit.Cryptography.Random.RandomGenerators
{
    internal interface IEntropyHasher : IDisposable
    {
        RandomNumberGenerator Rng { get; }
        IReadOnlyCollection<IHashAlgorithmWrapper> HashWrappers { get; }
    }
}