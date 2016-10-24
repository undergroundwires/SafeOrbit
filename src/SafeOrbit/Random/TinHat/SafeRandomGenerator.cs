
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using SafeOrbit.Random.Tinhat;
using SafeOrbit.Random.TinHat.Crypto.Digests;

namespace SafeOrbit.Random
{
    /// <summary>
    ///     TinHatRandom returns cryptographically strong random data, never to exceed the number of bytes available from
    ///     the specified entropy sources.  This can cause slow generation, and is recommended only for generating extremely
    ///     strong keys and other things that don't require a large number of bytes quickly.  This is CPU intensive.  For
    ///     general
    ///     purposes, see TinHatURandom instead.
    /// </summary>
    /// <remarks>
    ///     TinHatRandom returns cryptographically strong random data, never to exceed the number of bytes available from
    ///     the specified entropy sources.  This can cause slow generation, and is recommended only for generating extremely
    ///     strong keys and other things that don't require a large number of bytes quickly.  This is CPU intensive.  For
    ///     general
    ///     purposes, see TinHatURandom instead.
    /// </remarks>
    /// <example>
    ///     <code>
    ///  using tinhat;
    ///  
    ///  static void Main(string[] args)
    ///  {
    ///      StartEarly.StartFillingEntropyPools();  // Start gathering entropy as early as possible
    ///  
    ///      var randomBytes = new byte[32];
    /// 
    ///      // Performance is highly variable.  On my system it generated 497Bytes(minimum)/567KB(avg)/1.7MB(max) per second
    ///      // default TinHatRandom() constructor uses:
    ///      //     SystemRNGCryptoServiceProvider/SHA256, 
    ///      //     ThreadedSeedGeneratorRNG/SHA256/RipeMD256Digest,
    ///      //     (if available) EntropyFileRNG/SHA256
    ///      TinHatRandom.StaticInstance.GetBytes(randomBytes);
    ///  }
    ///  </code>
    /// </example>
    public sealed class SafeRandomGenerator : RandomNumberGenerator
    {
        // Interlocked cannot handle bools.  So using int as if it were bool.
        private const int TrueInt = 1;
        private const int FalseInt = 0;
        private static readonly Lazy<SafeRandomGenerator> StaticInstanceLazy = new Lazy<SafeRandomGenerator>();
        private List<IEntropyHasher> _entropyHashers;
        private int _hashLengthInBytes;
        private int _isDisposed = FalseInt;

        public SafeRandomGenerator()
        {
            _entropyHashers = new List<IEntropyHasher>();

            // Add the .NET implementation of SHA256 and RNGCryptoServiceProvider
            {
                var rng = new SystemRngCryptoServiceProvider();
                var hashWrapper = new HashAlgorithmWrapper(SHA256.Create());
                _entropyHashers.Add(new EntropyHasher(rng, hashWrapper));
            }

            // Add the ThreadedSeedGeneratorRNG as entropy source, and chain SHA256 and RipeMD256 as hash algorithms
            {
                var rng = new ThreadedSeedGeneratorRng();
                var hashWrappers = new List<HashAlgorithmWrapper>
                {
                    new HashAlgorithmWrapper(SHA256.Create()),
                    new HashAlgorithmWrapper(new RipeMD256Digest())
                };
                _entropyHashers.Add(new EntropyHasher(rng, hashWrappers));
            }

            // Add the ThreadSchedulerRNG as entropy source, and SHA256 as hash algorithm
            {
                var rng = new ThreadSchedulerRng();
                var hashWrapper = new HashAlgorithmWrapper(new Sha256Digest());
                _entropyHashers.Add(new EntropyHasher(rng, hashWrapper));
            }

            // If available, add EntropyFileRNG as entropy source
            {
                EntropyFileRng rng = null;
                try
                {
                    rng = new EntropyFileRng();
                }
                catch
                {
                    // EntropyFileRNG thows exceptions if it hasn't been seeded yet, if it encouters corruption, etc.
                }
                if (rng != null)
                {
                    var hashWrapper = new HashAlgorithmWrapper(SHA256.Create());
                    _entropyHashers.Add(new EntropyHasher(rng, hashWrapper));
                }
            }

            CtorSanityCheck();
        }

        public SafeRandomGenerator(List<IEntropyHasher> entropyHashers)
        {
            _entropyHashers = entropyHashers;
            CtorSanityCheck();
        }

        public static SafeRandomGenerator StaticInstance => StaticInstanceLazy.Value;

        private void CtorSanityCheck()
        {
            if (_entropyHashers == null)
                throw new ArgumentNullException("_entropyHashers");
            if (_entropyHashers.Count < 1)
                throw new ArgumentException("EntropyHashers.Count cannot be < 1");
            var hashLengthInBits = -1;
            lock (_entropyHashers)
            {
                foreach (var eHasher in _entropyHashers)
                {
                    if (eHasher.Rng == null)
                        throw new ArgumentException("RNG cannot be null");
                    if (eHasher.HashWrappers == null)
                        throw new ArgumentException("HashWrappers cannot be null");
                    if (eHasher.HashWrappers.Count < 1)
                        throw new ArgumentException("HashWrappers.Count cannot be < 1");
                    foreach (var hashWrapper in eHasher.HashWrappers)
                        if (hashLengthInBits == -1)
                        {
                            hashLengthInBits = hashWrapper.HashSizeInBits;
                        }
                        else
                        {
                            if (hashLengthInBits != hashWrapper.HashSizeInBits)
                                throw new ArgumentException("Hash functions must all return the same size digest");
                        }
                }
            }
            _hashLengthInBytes = hashLengthInBits/8;
        }

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
            else // if (byteArrays.Count > 1)
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
            if ((first == null) || (second == null))
                throw new CryptographicException("null byte array in allByteArraysThatMustBeUnique");
            if ((first.Length != _hashLengthInBytes) || (second.Length != _hashLengthInBytes))
                throw new CryptographicException("byte array in allByteArraysThatMustBeUnique with wrong length");
            for (var i = 0; i < _hashLengthInBytes; i++)
                if (first[i] != second[i])
                    return false;
            return true;
        }

        public override void GetBytes(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length == 0)
                return;
            var pos = 0;
            var finished = false;
            while (false == finished)
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
                        if ((eHasher.HashWrappers == null) || (eHasher.HashWrappers.Count < 1))
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
                            throw new Exception("Impossible Exception # A0B276734D");
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

        public override void GetNonZeroBytes(byte[] data)
        {
            // Apparently, the reason for GetNonZeroBytes to exist, is sometimes people generate null-terminated salt strings.
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length == 0)  return;
            var pos = 0;
            while (true)
            {
                var tempData = new byte[(int) (1.05*(data.Length - pos))];
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

        protected override void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref _isDisposed, TrueInt) == TrueInt)
                return;
            if (_entropyHashers != null)
            {
                List<IEntropyHasher> myHashers = _entropyHashers;
                _entropyHashers = null;
                try
                {
                    foreach (var hasher in myHashers)
                        try
                        {
                            ((IDisposable) hasher).Dispose();
                        }
                        catch
                        {
                        }
                }
                catch
                {
                }
            }
            base.Dispose(disposing);
        }

        ~SafeRandomGenerator()
        {
            Dispose(false);
        }
    }
}