using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using SafeOrbit.Random.Tinhat;

namespace SafeOrbit.Random
{
    public sealed class EntropyHasher : IEntropyHasher
    {
        public RandomNumberGenerator Rng { get; private set; }
        public IList<HashAlgorithmWrapper> HashWrappers { get; private set; }
        public EntropyHasher(RandomNumberGenerator rng, HashAlgorithmWrapper hashWrapper)
        {
            if (rng == null) throw new ArgumentNullException(nameof(rng));
            if (hashWrapper == null) throw new ArgumentNullException(nameof(hashWrapper));
            this.Rng = rng;
            this.HashWrappers = new List<HashAlgorithmWrapper> {hashWrapper};
        }
        public EntropyHasher(RandomNumberGenerator rng, List<HashAlgorithmWrapper> hashWrappers)
        {
            if (rng == null) throw new ArgumentNullException(nameof(rng));
            if (hashWrappers == null) throw new ArgumentNullException(nameof(hashWrappers));
            this.Rng = rng;
            this.HashWrappers = hashWrappers;
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
                if (HashWrappers != null)
                {
                    try
                    {
                        foreach (HashAlgorithmWrapper hashWrapper in HashWrappers)
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
        }
        ~EntropyHasher()
        {
            Dispose();
        }
    }
}