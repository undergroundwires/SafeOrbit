#if !NETCORE
using System;
using System.Security.Cryptography;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
{
    /// <summary>
    ///     .NET Framework implementation of <see cref="IByteArrayProtector" />.
    ///     Wrapper for <see cref="ProtectedMemory" />.
    /// </summary>
    /// <seealso cref="IByteArrayProtector" />
    public partial class MemoryProtector : IByteArrayProtector
    {
        public int BlockSizeInBytes => 16;

        public void Protect(byte[] userData)
        {
            this.EnsureParameter(userData);
            if (userData == null) throw new ArgumentNullException(nameof(userData));
            ProtectedMemory.Protect(userData, MemoryProtectionScope.SameProcess);
        }

        public void Unprotect(byte[] encryptedData)
        {
            this.EnsureParameter(encryptedData);
            if (encryptedData == null) throw new ArgumentNullException(nameof(encryptedData));
            ProtectedMemory.Unprotect(encryptedData, MemoryProtectionScope.SameProcess);
        }
    }
}
#endif