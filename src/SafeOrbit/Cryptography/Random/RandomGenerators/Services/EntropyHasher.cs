using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using SafeOrbit.Common;
using SafeOrbit.Extensions;

namespace SafeOrbit.Cryptography.Random.RandomGenerators
{
    internal sealed class EntropyHasher : DisposableBase, IEntropyHasher
    {
        /// <exception cref="ArgumentNullException"><paramref name="hashWrapper" /> is <see langword="null" />.</exception>
        public EntropyHasher(RandomNumberGenerator rng, IHashAlgorithmWrapper hashWrapper)
            : this(rng, new[] {hashWrapper})
        {
            if (hashWrapper == null) throw new ArgumentNullException(nameof(hashWrapper));
        }

        /// <exception cref="ArgumentNullException"><paramref name="rng" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="hashWrappers" /> is <see langword="null" />.</exception>
        public EntropyHasher(RandomNumberGenerator rng, IReadOnlyCollection<IHashAlgorithmWrapper> hashWrappers)
        {
            Rng = rng ?? throw new ArgumentNullException(nameof(rng));
            HashWrappers = hashWrappers ?? throw new ArgumentNullException(nameof(hashWrappers));
        }

        public RandomNumberGenerator Rng { get; private set; }
        public IReadOnlyCollection<IHashAlgorithmWrapper> HashWrappers { get; private set; }

        protected override void DisposeManagedResources()
        {
            Rng?.Dispose();
            foreach(var hashWrapper in HashWrappers.EmptyIfNull())
                hashWrapper?.Dispose();
        }
    }
}