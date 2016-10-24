using System;
using System.Security.Cryptography;

namespace SafeOrbit.Random
{
    /// <summary>
    /// A wrapper around <see cref="RNGCryptoServiceProvider" />
    /// </summary>
    /// <seealso cref="RandomNumberGenerator" />
    public class SafeRandom : ISafeRandom
    {
        private readonly RandomNumberGenerator _generator;
        public SafeRandom() : this(new RNGCryptoServiceProvider()) { }
        internal SafeRandom(RNGCryptoServiceProvider generator   )
        {
            _generator = generator;
        }
        public void Seed(byte[] seed)
        {
            throw new NotImplementedException();
        }

        public byte[] GetBytes(int length)
        {
            var data = new byte[length];
            _generator.GetBytes(data);
            return data;
        }

        public int GetInt()
        {
            throw new NotImplementedException();
        }

        public byte[] GetNonZeroBytes(int length)
        {
            throw new NotImplementedException();
        }


        public int GetInt(int min, int max)
        {
            throw new NotImplementedException();
        }

        public bool GetBool()
        {
            throw new NotImplementedException();
        }
    }
}
