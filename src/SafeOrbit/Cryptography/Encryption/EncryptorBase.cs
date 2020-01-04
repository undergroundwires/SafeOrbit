using System;
using SafeOrbit.Cryptography.Encryption.Padding;
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

        /// <exception cref="ArgumentNullException"><paramref name="random" /> is <see langword="null" />.</exception>
        protected EncryptorBase(ICryptoRandom random)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        /// <inheritdoc />
        public abstract int MinKeySizeInBits { get; }

        /// <inheritdoc />
        /// <remarks>https://en.wikipedia.org/wiki/Block_size_(cryptography)</remarks>
        public abstract int BlockSizeInBits { get; }

        /// <inheritdoc />
        public abstract int MaxKeySizeInBits { get; }

        public abstract int IvSizeInBits { get; }

        /// <summary>
        ///     Ensures that the <paramref name="key" /> is not null and its size is between <see cref="MinKeySizeInBits" /> and <see cref="MaxKeySizeInBits" />
        /// </summary>
        /// <param name="key">The key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is <see langword="null" />.</exception>
        /// <exception cref="KeySizeException">
        ///     Length of the <paramref name="key" /> must be between <see cref="MinKeySizeInBits" /> and <see cref="MaxKeySizeInBits" />values.
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