using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SafeOrbit.Cryptography.Random.RandomGenerators
{
    internal sealed class EntropyHasher : IEntropyHasher
    {
        public RandomNumberGenerator Rng { get; private set; }
        public IReadOnlyCollection<IHashAlgorithmWrapper> HashWrappers { get; private set; }

        /// <exception cref="ArgumentNullException"><paramref name="hashWrapper" /> is <see langword="null" />.</exception>
        public EntropyHasher(RandomNumberGenerator rng, IHashAlgorithmWrapper hashWrapper)
            :this(rng, new[] { hashWrapper })
        {
            if (hashWrapper == null) throw new ArgumentNullException(nameof(hashWrapper));
        }

        /// <exception cref="ArgumentNullException"><paramref name="rng" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="hashWrappers" /> is <see langword="null" />.</exception>
        public EntropyHasher(RandomNumberGenerator rng, IReadOnlyCollection<IHashAlgorithmWrapper> hashWrappers)
        {
            this.Rng = rng ?? throw new ArgumentNullException(nameof(rng));
            this.HashWrappers = hashWrappers ?? throw new ArgumentNullException(nameof(hashWrappers));
        }
        public void Dispose()
        {
            lock (this)
            {
                if (Rng != null)
                {
                    try
                    {
                        Rng.Dispose();
                    }
                    catch { /* Continue cleanup */ }
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
                        catch { /* Continue cleanup */ }
                    }
                }
                catch { /* Continue cleanup */ }
                HashWrappers = null;
            }
        }
        ~EntropyHasher() => Dispose();
    }
}