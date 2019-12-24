﻿using System;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector
{
    /// <inheritdoc />
    /// <summary>
    ///     Encrypts/decrypt a byte array using <see cref="T:SafeOrbit.Cryptography.Encryption.BlowfishEcb" />.
    /// </summary>
    /// <seealso cref="T:SafeOrbit.Memory.SafeBytesServices.DataProtection.IByteArrayProtector" />
    public partial class MemoryProtector
    {
        private void EnsureParameter(byte[] userData)
        {
            if (userData == null) throw new ArgumentNullException(nameof(userData));
            if (userData.Length % BlockSizeInBytes != 0)
                throw new ArgumentOutOfRangeException($"Size of {nameof(userData)} must be" +
                                                      $"multiple of {BlockSizeInBytes}");
        }
    }
}