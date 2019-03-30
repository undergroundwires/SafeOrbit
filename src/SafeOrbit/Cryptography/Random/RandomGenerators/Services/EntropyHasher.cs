using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SafeOrbit.Cryptography.Random.RandomGenerators
{
    internal sealed class EntropyHasher : IEntropyHasher
    {
        public RandomNumberGenerator Rng { get; private set; }
        public IReadOnlyCollection<IHashAlgorithmWrapper> HashWrappers { get; private set; }
        public EntropyHasher(RandomNumberGenerator rng, IHashAlgorithmWrapper hashWrapper)
        {
            if (hashWrapper == null) throw new ArgumentNullException(nameof(hashWrapper));
            this.Rng = rng ?? throw new ArgumentNullException(nameof(rng));
            this.HashWrappers = new []{ hashWrapper};
        }
        public EntropyHasher(RandomNumberGenerator rng, IReadOnlyCollection<IHashAlgorithmWrapper> hashWrappers)
        {
            this.Rng = rng ?? throw new ArgumentNullException(nameof(rng));
            this.HashWrappers = hashWrappers ?? throw new ArgumentNullException(nameof(hashWrappers));
        }
        public void Dispose()
        {
            lock (this)      // Just in case two threads try to dispose me at the same time?  Whatev.  ;-)
            {
                if (Rng != null)
                {
                    try
                    {
                        Rng.Dispose();
                    }
                    catch { }
                    Rng = null;
                }
                if (HashWrappers == null)
                    return;
                try
                {
                    foreach (var hashWrapper in HashWrappers)
                    {
                        try
                        {
                            hashWrapper.Dispose();
                        }
                        catch { }
                    }
                }
                catch { }
                HashWrappers = null;
            }
        }
        ~EntropyHasher() => Dispose();
    }
}