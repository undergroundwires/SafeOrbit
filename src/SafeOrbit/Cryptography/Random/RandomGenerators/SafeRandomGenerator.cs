using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using SafeOrbit.Cryptography.Random.RandomGenerators.Crypto.Digests;
using SafeOrbit.Extensions;
using SafeOrbit.Threading;

namespace SafeOrbit.Cryptography.Random.RandomGenerators
{
    /// <inheritdoc />
    /// <summary>
    ///     <see cref="T:SafeOrbit.Cryptography.Random.RandomGenerators.SafeRandomGenerator" /> returns cryptographically
    ///     strong random data, never to exceed the number of
    ///     bytes available from the specified entropy sources.  This can cause slow generation, and is recommended only for
    ///     generating extremely strong keys and other things that don't require a large number of bytes quickly.  This is CPU
    ///     intensive. For general purposes, see
    ///     <see cref="T:SafeOrbit.Cryptography.Random.RandomGenerators.FastRandomGenerator" /> instead.
    /// </summary>
    /// <example>
    ///     <code>
    ///           using SafeOrbit.Cryptography.Random.RandomGenerators;
    ///           static void Main(string[] args)
    ///           {
    ///           StartEarly.StartFillingEntropyPools();  // Start gathering entropy as early as possible
    ///                 ///      var randomBytes = new byte[32];
    ///                 // Performance is highly variable.  On my system it generated 497Bytes(minimum)/567KB(avg)/1.7MB(max) per second
    ///                 // default SafeRandomGenerator() constructor uses:
    ///                 //     SystemRNGCryptoServiceProvider/SHA256, 
    ///                 //     ThreadedSeedGeneratorRNG/SHA256/RipeMD256Digest
    ///      SafeRandomGenerator.StaticInstance.GetBytes(randomBytes);
    ///  }
    ///  </code>
    /// </example>
    public sealed class SafeRandomGenerator : RandomNumberGenerator
    {
        private static readonly Lazy<SafeRandomGenerator> StaticInstanceLazy = new Lazy<SafeRandomGenerator>();
        private readonly int _hashLengthInBytes;
        private IReadOnlyCollection<IEntropyHasher> _entropyHashers;

        private int
            _isDisposed = IntCondition.False; // Interlocked cannot handle bools. So using int as if it were bool.

        public SafeRandomGenerator() : this(GetEntropyHashers().ToArray())
        {
        }

        internal SafeRandomGenerator(IReadOnlyCollection<IEntropyHasher> entropyHashers)
        {
            ValidateEntropyHashers(entropyHashers);
            _entropyHashers = entropyHashers;
            _hashLengthInBytes = GetHashLengthInBits(entropyHashers) / 8;
        }

        public static SafeRandomGenerator StaticInstance => StaticInstanceLazy.Value;
        ~SafeRandomGenerator() => Dispose(false);

        public override void GetBytes(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length == 0)
                return;
            var pos = 0;
            var finished = false;
            while (!finished)
            {
                var allByteArraysThatMustBeUnique = new List<byte[]>();
                var outputs = new List<byte[]>();
                lock (_entropyHashers)
                {
                    foreach (var eHasher in _entropyHashers)
                    {
                        var hashes = new List<byte[]>();
                        var entropy = new byte[_hashLengthInBytes];
                        eHasher.Rng.GetBytes(entropy);
                        allByteArraysThatMustBeUnique.Add(entropy);
                        if (eHasher.HashWrappers == null || eHasher.HashWrappers.Count < 1)
                            throw new CryptographicException(
                                "eHasher.HashWrappers == null || eHasher.HashWrappers.Count < 1");
                        foreach (var hasher in eHasher.HashWrappers)
                        {
                            var hash = hasher.ComputeHash(entropy);
                            hashes.Add(hash);
                            allByteArraysThatMustBeUnique.Add(hash);
                        }

                        // We don't bother comparing any of the hashes for equality right now, because the big loop
                        // will do that later, when checking allByteArraysThatMustBeUnique.
                        if (hashes.Count == 1)
                        {
                            // We don't need to combine hashes, if there is only one hash.
                            // No need to allByteArraysThatMustBeUnique.Add, because that was already done above.
                            outputs.Add(hashes[0]);
                        }
                        else if (hashes.Count > 1)
                        {
                            var output = CombineByteArrays(hashes);
                            allByteArraysThatMustBeUnique.Add(output);
                            outputs.Add(output);
                        }
                        else
                        {
                            // Impossible to get here because foreach() loops over eHasher.HashWrappers and does "hashes.Add" on each
                            // iteration.  And eHasher.HashWrappers was already checked for null and checked for Count < 1
                            throw new ArgumentException("Impossible Exception # A0B276734D");
                        }
                    }
                }

                var finalOutput = CombineByteArrays(outputs);
                allByteArraysThatMustBeUnique.Add(finalOutput);
                for (var i = 0; i < allByteArraysThatMustBeUnique.Count - 1; i++)
                {
                    var firstByteArray = allByteArraysThatMustBeUnique[i];
                    for (var j = i + 1; j < allByteArraysThatMustBeUnique.Count; j++)
                    {
                        var secondByteArray = allByteArraysThatMustBeUnique[j];
                        if (CompareByteArrays(firstByteArray, secondByteArray))
                            throw new CryptographicException("non-unique arrays in allByteArraysThatMustBeUnique");
                    }
                }

                for (var i = 0; i < finalOutput.Length; i++) // copy the finalOutput to the requested user buffer
                {
                    data[pos] = finalOutput[i];
                    pos++;
                    if (pos == data.Length)
                    {
                        finished = true;
                        break;
                    }
                }

                foreach (var byteArray in allByteArraysThatMustBeUnique)
                    Array.Clear(byteArray, 0, byteArray.Length);
            }
        }

#if !NETSTANDARD1_6
        public override void GetNonZeroBytes(byte[] data)
        {
            // Apparently, the reason for GetNonZeroBytes to exist, is sometimes people generate null-terminated salt strings.
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length == 0) return;
            var pos = 0;
            while (true)
            {
                var tempData = new byte[(int) (1.05 * (data.Length - pos))];
                // Request 5% more data than needed, to reduce the probability of repeating loop
                GetBytes(tempData);
                for (var i = 0; i < tempData.Length; i++)
                    if (tempData[i] != 0)
                    {
                        data[pos] = tempData[i];
                        pos++;
                        if (pos == data.Length)
                        {
                            Array.Clear(tempData, 0, tempData.Length);
                            return;
                        }
                    }
            }
        }
#endif


        private byte[] CombineByteArrays(List<byte[]> byteArrays)
        {
            if (byteArrays == null) throw new ArgumentNullException(nameof(byteArrays));
            if (byteArrays.Count < 1) throw new ArgumentException("byteArrays.Count < 1");

            var accumulator = new byte[_hashLengthInBytes];
            if (byteArrays.Count == 1)
            {
                if (byteArrays[0].Length != _hashLengthInBytes)
                    throw new ArgumentException("byteArray.Length != HashLengthInBytes");
                Array.Copy(byteArrays[0], accumulator, _hashLengthInBytes);
            }
            else // byteArrays.Count > 1
            {
                Array.Clear(accumulator, 0, accumulator.Length); // Should be unnecessary, but just to make sure.
                foreach (var byteArray in byteArrays)
                {
                    if (byteArray.Length != _hashLengthInBytes)
                        throw new ArgumentException("byteArray.Length != HashLengthInBytes");
                    for (var i = 0; i < _hashLengthInBytes; i++)
                        accumulator[i] ^= byteArray[i];
                }
            }

            return accumulator;
        }

        private bool CompareByteArrays(byte[] first, byte[] second)
        {
            if (first == null || second == null)
                throw new CryptographicException("null byte array in allByteArraysThatMustBeUnique");
            if (first.Length != _hashLengthInBytes || second.Length != _hashLengthInBytes)
                throw new CryptographicException("byte array in allByteArraysThatMustBeUnique with wrong length");
            for (var i = 0; i < _hashLengthInBytes; i++)
                if (first[i] != second[i])
                    return false;
            return true;
        }


        protected override void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref _isDisposed, IntCondition.True) == IntCondition.True)
                return;
            if (_entropyHashers != null)
            {
                var myHashers = _entropyHashers;
                _entropyHashers = null;
                try
                {
                    foreach (var hasher in myHashers)
                        try
                        {
                            hasher.Dispose();
                        }
                        catch
                        {
                            /* Swallow */
                        }
                }
                catch
                {
                    /* Swallow */
                }
            }

            base.Dispose(disposing);
        }

        private static IEnumerable<IEntropyHasher> GetEntropyHashers()
        {
            yield return GetHasher(
                rng: new SystemRng(),
                algorithms: new HashAlgorithmWrapper(SHA256.Create()));
            yield return GetHasher(
                new ThreadedSeedGeneratorRng(),
                /* Algorithms */
                new HashAlgorithmWrapper(SHA256.Create()),
                new HashAlgorithmWrapper(new RipeMD256Digest()));
            yield return GetHasher(
                new ThreadSchedulerRng(),
                /* Algorithms */
                new HashAlgorithmWrapper(new Sha256Digest()));

            static IEntropyHasher GetHasher(RandomNumberGenerator rng, params IHashAlgorithmWrapper[] algorithms)
                => new EntropyHasher(rng, algorithms);
        }

        private static int GetHashLengthInBits(IEnumerable<IEntropyHasher> entropyHashers)
        {
            var hashSizes = entropyHashers
                .SelectMany(e => e.HashWrappers)
                .Select(w => w.HashSizeInBits)
                .ToArray();
            if (!hashSizes.AreAllEqual())
                throw new ArgumentException("Hash functions must all return the same size digest");
            return hashSizes.First();
        }

        private static void ValidateEntropyHashers(IReadOnlyCollection<IEntropyHasher> entropyHashers)
        {
            if (entropyHashers == null) throw new ArgumentNullException(nameof(entropyHashers));
            if (entropyHashers.Count < 1) throw new ArgumentException($"{nameof(entropyHashers.Count)} cannot be < 1");
            lock (entropyHashers)
            {
                foreach (var hasher in entropyHashers) ValidateEntropyHasher(hasher);
            }
        }

        private static void ValidateEntropyHasher(IEntropyHasher eHasher)
        {
            if (eHasher == null) throw new ArgumentNullException(nameof(eHasher));
            if (eHasher.Rng == null)
                throw new ArgumentException($"{nameof(eHasher.Rng)} property is null for the entropy hasher.");
            if (eHasher.HashWrappers == null)
                throw new ArgumentException($"{nameof(eHasher.HashWrappers)} property is null for the entropy hasher.");
            if (eHasher.HashWrappers.Count < 1)
                throw new ArgumentException($"{nameof(eHasher.HashWrappers)} property must at least have one element.");
        }
    }
}