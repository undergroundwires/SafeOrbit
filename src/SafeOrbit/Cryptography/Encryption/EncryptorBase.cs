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
        public abstract int MinKeySizeInBits { get; }

        /// <summary>
        /// Gets the size of the block in bits.
        /// </summary>
        /// <value>The size of the block in bits.</value>
        /// <remarks>https://en.wikipedia.org/wiki/Block_size_(cryptography)</remarks>
        public abstract int BlockSizeInBits { get; }

        /// <summary>
        ///     Maximum size of key in bits for algorithm to function.
        /// </summary>
        /// <value>The maximum size of the key.</value>
        public abstract int MaxKeySizeInBits { get; }

        public abstract int IvSizeInBits { get; }

        /// <summary>
        ///     Ensures that the <paramref name="key" /> is not null and its size is between <see cref="MinKeySizeInBits" /> and
        ///     <see cref="MaxKeySizeInBits" />
        /// </summary>
        /// <param name="key">The key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is <see langword="null" />.</exception>
        /// <exception cref="KeySizeException">
        ///     Length of the <paramref name="key" /> must be between <see cref="MinKeySizeInBits" /> and
        ///     <see cref="MaxKeySizeInBits" />values.
        /// </exception>
        /// <seealso cref="MinKeySizeInBits" />
        /// <seealso cref="MaxKeySizeInBits" />
        protected void ValidateKey(byte[] key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if ((key.Length < MinKeySizeInBits / 8) || (key.Length > MaxKeySizeInBits))
                throw new KeySizeException(minSize: 32, maxSize: 488, actual: key.Length);
        }

        protected byte[] GetIv() => _random.GetBytes(IvSizeInBits);
    }
}