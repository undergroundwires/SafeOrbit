using System.Threading.Tasks;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector
{
    /// <summary>
    ///     Encrypts and decrypts a <see cref="byte" /> array.
    ///     Use instead <see cref="IMemoryProtectedBytes" /> as it provides safer access to the inner bytes.
    /// </summary>
    /// <seealso cref="IMemoryProtectedBytes" />
    public interface IByteArrayProtector
    {
        /// <summary>
        ///     Gets the size of the blocks for encryption to function.
        /// </summary>
        /// <value>The size of the blocks.</value>
        int BlockSizeInBytes { get; }

        /// <summary>
        ///     Encrypts the specified user data.
        /// </summary>
        /// <param name="userData">The data to encrypt.</param>
        Task ProtectAsync(byte[] userData);

        /// <summary>
        ///     Decrypts the specified encrypted data.
        /// </summary>
        /// <param name="encryptedData">The encrypted data to decrypt.</param>
        Task UnprotectAsync(byte[] encryptedData);
    }
}