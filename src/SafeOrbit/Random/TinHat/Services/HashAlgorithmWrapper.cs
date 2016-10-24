using System;
using System.Security.Cryptography;
using SafeOrbit.Random.TinHat.Crypto;

namespace SafeOrbit.Random.Tinhat
{
    /// <summary>
    /// HashAlgorithmWrapper is an abstraction wrapper class, to contain either .NET System.Security.Cryptography.HashAlgorithm, 
    /// or Bouncy Castle Org.BouncyCastle.Crypto.IDigest, and make the user agnostic.
    /// </summary>
    public class HashAlgorithmWrapper : IDisposable
    {
        protected delegate byte[] ComputeHashDelegate(byte[] data);
        protected object HashAlgorithmObject;
        protected ComputeHashDelegate ComputeHashDelegateInstance;

        public int HashSizeInBits { get; protected set; }

        public HashAlgorithmWrapper(HashAlgorithm hashAlg)
        {
            HashAlgorithmObject = hashAlg;
            ComputeHashDelegateInstance = hashAlg.ComputeHash;
            HashSizeInBits = hashAlg.HashSize;     // HashAlg.HashSize is measured in bits
        }
        public HashAlgorithmWrapper(IDigest bciDigest)
        {
            HashAlgorithmObject = bciDigest;
            ComputeHashDelegateInstance = BouncyCastleComputeHashDelegateProvider;
            HashSizeInBits = bciDigest.GetDigestSize() * 8;   // GetDigestSize() returns a number of bytes
        }

        public byte[] ComputeHash(byte[] data)
        {
            return this.ComputeHashDelegateInstance(data);
        }

        protected byte[] BouncyCastleComputeHashDelegateProvider(byte[] data)
        {
            IDigest bciDigest = (IDigest)this.HashAlgorithmObject;
            var output = new byte[bciDigest.GetDigestSize()];
            bciDigest.BlockUpdate(data, 0, data.Length);
            bciDigest.DoFinal(output, 0);
            bciDigest.Reset();
            return output;
        }

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (this.HashAlgorithmObject != null)
                {
                    try
                    {
                        if (this.HashAlgorithmObject is IDisposable)
                        {
                            ((IDisposable)this.HashAlgorithmObject).Dispose();
                        }
                    }
                    catch { }
                    try
                    {
                        if (this.HashAlgorithmObject is IDigest)
                        {
                            ((IDigest)this.HashAlgorithmObject).Reset();
                        }
                    }
                    catch { }
                    HashAlgorithmObject = null;
                }
            }
        }
        ~HashAlgorithmWrapper()
        {
            Dispose(false);
        }
    }
}