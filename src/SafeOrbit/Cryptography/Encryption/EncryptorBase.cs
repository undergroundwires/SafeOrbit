
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
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Exceptions;

namespace SafeOrbit.Cryptography.Encryption
{
    /// <summary>
    ///     Base class for the encryptors.
    /// </summary>
    /// <seealso cref="IEncryptor" />
    /// <seealso cref="AesEncryptor"/>
    /// <seealso cref="BlowfishEncryptor"/>
    public abstract class EncryptorBase : IEncryptor
    {
        private readonly ICryptoRandom _random;

        protected EncryptorBase(ICryptoRandom random)
        {
            if (random == null) throw new ArgumentNullException(nameof(random));
            _random = random;
        }

        /// <summary>
        ///     Minimum size of key in bits for algorithm to function.
        /// </summary>
        /// <value>The minimum size of the key.</value>
        public abstract int MinKeySize { get; }

        /// <summary>
        /// Gets the size of the block in bits.
        /// </summary>
        /// <value>The size of the block in bits.</value>
        /// <remarks>https://en.wikipedia.org/wiki/Block_size_(cryptography)</remarks>
        public abstract int BlockSize { get; }

        /// <summary>
        ///     Maximum size of key in bits for algorithm to function.
        /// </summary>
        /// <value>The maximum size of the key.</value>
        public abstract int MaxKeySize { get; }

        public abstract int IvSize { get; }

        /// <summary>
        ///     Ensures that the <paramref name="key" /> is not null and its size is between <see cref="MinKeySize" /> and
        ///     <see cref="MaxKeySize" />
        /// </summary>
        /// <param name="key">The key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is <see langword="null" />.</exception>
        /// <exception cref="KeySizeException">
        ///     Length of the <paramref name="key" /> must be between <see cref="MinKeySize" /> and
        ///     <see cref="MaxKeySize" />values.
        /// </exception>
        /// <seealso cref="MinKeySize" />
        /// <seealso cref="MaxKeySize" />
        protected void EnsureKeyParameter(byte[] key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if ((key.Length < MinKeySize/8) || (key.Length > MaxKeySize))
                throw new KeySizeException(minSize: 32, maxSize: 488, actual: key.Length);
        }

        protected byte[] GetIv() => _random.GetBytes(IvSize);
    }
}