using System;
using SafeOrbit.Exceptions;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector
{
    /// <inheritdoc />
    /// <summary>
    ///     Encrypts/decrypt a byte array using <see cref="Cryptography.Encryption.BlowfishEcb" />.
    /// </summary>
    /// <seealso cref="IByteArrayProtector" />
    public partial class MemoryProtector
    {
        /// <exception cref="ArgumentNullException"><paramref name="userData" /> is <see langword="null" />.</exception>
        /// <exception cref="DataLengthException"><paramref name="userData" /> has not the size of the block.</exception>
        private void EnsureParameter(byte[] userData)
        {
            if (userData == null) throw new ArgumentNullException(nameof(userData));
            if (userData.Length % BlockSizeInBytes != 0)
                throw new DataLengthException(nameof(userData), $"Size of {nameof(userData)} must be" +
                                                                $"multiple of {BlockSizeInBytes}");
        }
    }
}