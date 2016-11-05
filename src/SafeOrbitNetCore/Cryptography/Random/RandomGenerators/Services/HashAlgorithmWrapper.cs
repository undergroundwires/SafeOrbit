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
using SafeOrbit.Cryptography.Random.RandomGenerators.Crypto;

namespace SafeOrbit.Cryptography.Random.RandomGenerators
{
    /// <summary>
    ///     HashAlgorithmWrapper is an abstraction wrapper class, to contain either .NET
    ///     System.Security.Cryptography.HashAlgorithm,
    ///     or Bouncy Castle Org.BouncyCastle.Crypto.IDigest, and make the user agnostic.
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

        public byte[] ComputeHash(byte[] data)
        {
            return ComputeHashDelegateInstance(data);
        }

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
                if (HashAlgorithmObject != null)
                {
                    try
                    {
                        if (HashAlgorithmObject is IDisposable)
                            ((IDisposable) HashAlgorithmObject).Dispose();
                    }
                    catch
                    {
                    }
                    try
                    {
                        if (HashAlgorithmObject is IDigest)
                            ((IDigest) HashAlgorithmObject).Reset();
                    }
                    catch
                    {
                    }
                    HashAlgorithmObject = null;
                }
            }
        }

        ~HashAlgorithmWrapper()
        {
            Dispose(false);
        }

        protected delegate byte[] ComputeHashDelegate(byte[] data);
    }
}