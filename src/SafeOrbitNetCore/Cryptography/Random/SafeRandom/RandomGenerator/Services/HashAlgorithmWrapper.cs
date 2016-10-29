
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
using System.Security.Cryptography;
using SafeOrbit.Random.TinHat.Crypto;

namespace SafeOrbit.Cryptography.Random.SafeRandomServices
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