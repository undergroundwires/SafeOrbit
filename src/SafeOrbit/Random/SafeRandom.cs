
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
