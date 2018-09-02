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
            _random = random ?? throw new ArgumentNullException(nameof(random));
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