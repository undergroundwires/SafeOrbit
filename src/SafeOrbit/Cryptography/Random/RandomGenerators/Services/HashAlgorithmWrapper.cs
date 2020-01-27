using System;
using System.Security.Cryptography;
using SafeOrbit.Common;
using SafeOrbit.Cryptography.Random.RandomGenerators.Crypto;

namespace SafeOrbit.Cryptography.Random.RandomGenerators
{
    /// <summary>
    ///     HashAlgorithmWrapper is an abstraction wrapper class, to contain either .NET
    ///     <see cref="HashAlgorithm" /> or <see cref="IDigest" /> and make the user agnostic.
    /// </summary>
    internal class HashAlgorithmWrapper : DisposableBase, IHashAlgorithmWrapper
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
            HashSizeInBits = bciDigest.GetDigestSize() * 8; // GetDigestSize() returns a number of bytes
        }

        public int HashSizeInBits { get; protected set; }


        /// <inheritdoc cref="ComputeHashDelegateInstance" />
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

        protected override void DisposeManagedResources()
        {
            switch (HashAlgorithmObject)
            {
                case null:
                    return;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
                case IDigest digest:
                    digest.Reset();
                    break;
            }

            HashAlgorithmObject = null;
        }

        protected delegate byte[] ComputeHashDelegate(byte[] data);
    }
}