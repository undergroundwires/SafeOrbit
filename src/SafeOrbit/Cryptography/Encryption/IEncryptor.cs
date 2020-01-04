namespace SafeOrbit.Cryptography.Encryption
{
    /// <summary>
    ///     An encryption algorithm.
    /// </summary>
    public interface IEncryptor
    {
        /// <summary>
        ///     Minimum size of key in bits for algorithm to function.
        /// </summary>
        /// <value>The minimum size of the key.</value>
        int MinKeySizeInBits { get; }

        /// <summary>
        ///     Maximum size of key in bits for algorithm to function.
        /// </summary>
        /// <value>The maximum size of the key.</value>
        int MaxKeySizeInBits { get; }

        /// <summary>
        ///     Gets the size of the iv bytes for crypto.
        /// </summary>
        /// <value>The size of the iv bytes that's created for crypto.</value>
        int IvSizeInBits { get; }

        /// <summary>
        ///     Gets the size of the block in bits.
        /// </summary>
        /// <remarks>https://en.wikipedia.org/wiki/Block_size_(cryptography)</remarks>
        /// <value>The size of the block in bits.</value>
        int BlockSizeInBits { get; }
    }
}