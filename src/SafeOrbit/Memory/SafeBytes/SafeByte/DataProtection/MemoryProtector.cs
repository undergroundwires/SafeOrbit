using System;
using System.Security.Cryptography;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    /// <summary>
    ///     A wrapper for <see cref="ProtectedMemory" />.
    /// </summary>
    /// <seealso cref="IByteArrayProtector" />
    /// <seealso cref="ProtectedMemory" />
    public class MemoryProtector : IByteArrayProtector
    {
        public int BlockSize => 16;

        public void Protect(byte[] userData)
        {
            if (userData == null) throw new ArgumentNullException(nameof(userData));
            if (userData.Length%BlockSize != 0)
                throw new ArgumentOutOfRangeException($"Size of {nameof(userData)} must be {BlockSize}");
            ProtectedMemory.Protect(userData, MemoryProtectionScope.SameProcess);
        }

        public void Unprotect(byte[] encryptedData)
        {
            if (encryptedData == null) throw new ArgumentNullException(nameof(encryptedData));
            if (encryptedData.Length%BlockSize != 0)
                throw new ArgumentOutOfRangeException($"Size of {nameof(encryptedData)} must be {BlockSize}");
            ProtectedMemory.Unprotect(encryptedData, MemoryProtectionScope.SameProcess);
        }
    }
}