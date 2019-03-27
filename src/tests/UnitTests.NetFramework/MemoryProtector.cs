using System;
using SafeOrbit.Cryptography.Encryption;
#if NET46
using System.Security.Cryptography;
#endif
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
#if NET46
            ProtectedMemory.Protect(userData, MemoryProtectionScope.SameProcess);
#endif
        }

        public void Unprotect(byte[] encryptedData)
        {
            if (encryptedData == null) throw new ArgumentNullException(nameof(encryptedData));
            if (encryptedData.Length%BlockSize != 0)
                throw new ArgumentOutOfRangeException($"Size of {nameof(encryptedData)} must be {BlockSize}");
       #if NET46
     ProtectedMemory.Unprotect(encryptedData, MemoryProtectionScope.SameProcess);
#endif
        }
    }
}