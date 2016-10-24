namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    /// <summary>
    ///     Encrypts and decrypts a <see cref="byte" /> array.
    /// </summary>
    public interface IByteArrayProtector
    {
        /// <summary>
        /// Gets the size of the blocks for encryption to function.
        /// </summary>
        /// <value>The size of the blocks.</value>
        int BlockSize { get; }
        /// <summary>
        /// Encrypts the specified user data.
        /// </summary>
        /// <param name="userData">The data to encrypt.</param>
        void Protect(byte[] userData);
        /// <summary>
        /// Decrypts the specified encrypted data.
        /// </summary>
        /// <param name="encryptedData">The encrypted data to decrypt.</param>
        void Unprotect(byte[] encryptedData);
    }
}