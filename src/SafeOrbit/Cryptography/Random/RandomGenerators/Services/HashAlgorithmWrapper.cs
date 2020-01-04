using System;
using System.Security.Cryptography;
using SafeOrbit.Cryptography.Random.RandomGenerators.Crypto;

namespace SafeOrbit.Cryptography.Random.RandomGenerators
{
    /// <summary>
    ///     HashAlgorithmWrapper is an abstraction wrapper class, to contain either .NET
    ///     <see cref="HashAlgorithm"/> or <see cref="IDigest"/> and make the user agnostic.
    /// </summary>
    internal class HashAlgorithmWrapper : IHashAlgorithmWrapper
    {
        protected ComputeHashDelegate ComputeHashDelegateInstance;
        protected object HashAlgorithmObject;

        public HashAlgorithmWrapper(HashAlgorithm hashAlg)
        {
            HashAlgorithmObject = hashAlg;
            ComputeHashDelegateInstance = hashAlg.ComputeHash;
            HashSizeInBits = hashAlg.HashSize; // HashAlg.HashSize is measured in bits
        }

        public HashAlgorithmWrapper(IDigest bciDigest)
        {
            HashAlgorithmObject = bciDigest;
            ComputeHashDelegateInstance = BouncyCastleComputeHashDelegateProvider;
            HashSizeInBits = bciDigest.GetDigestSize()*8; // GetDigestSize() returns a number of bytes
        }

        public int HashSizeInBits { get; protected set; }

        public void Dispose()
        {
            Dispose(true);
        }

        /// <inheritdoc cref="ComputeHashDelegateInstance"/>
        public byte[] ComputeHash(byte[] data) => ComputeHashDelegateInstance(data);

        protected byte[] BouncyCastleComputeHashDelegateProvider(byte[] data)
        {
            var bciDigest = (IDigest) HashAlgorithmObject;
            var output = new byte[bciDigest.GetDigestSize()];
            bciDigest.BlockUpdate(data, 0, data.Length);
            bciDigest.DoFinal(output, 0);
            bciDigest.Reset();
            return output;
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (HashAlgorithmObject == null)
                    return; 
                if (HashAlgorithmObject is IDisposable disposable) 
                    disposable.Dispose();
                else if (HashAlgorithmObject is IDigest digest) 
                    digest.Reset();
                HashAlgorithmObject = null;
            }
        }

        ~HashAlgorithmWrapper()
        {
            Dispose(false);
        }

        protected delegate byte[] ComputeHashDelegate(byte[] data);
    }
}