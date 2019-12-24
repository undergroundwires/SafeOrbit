#if NETSTANDARD1_6 || NETSTANDARD2
using System;
using SafeOrbit.Cryptography.Encryption;
using SafeOrbit.Cryptography.Random;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector
{
    /// <summary>
    ///     .NET Core implementation of <see cref="IByteArrayProtector" />.
    /// </summary>
    /// <seealso cref="IByteArrayProtector" />
    public partial class MemoryProtector : IByteArrayProtector
    {
        private static readonly IFastEncryptor Encryptor = new BlowfishEncryptor(BlowfishCipherMode.Ecb);
        private readonly IFastEncryptor _encryptor;
        private readonly byte[] _key;

        public MemoryProtector() : this(Encryptor, FastRandom.StaticInstance)
        {
        }

        internal MemoryProtector(IFastEncryptor encryptor, ICryptoRandom random)
        {
            _encryptor = encryptor;
            _key = random.GetBytes(Encryptor.MinKeySize);
        }

        public int BlockSizeInBytes => _encryptor.BlockSize/4;

        public void Protect(byte[] userData)
        {
            this.EnsureParameter(userData);
            var encryptedBytes = _encryptor.Encrypt(userData, _key);
            SetBytesToByteArray(
                source: encryptedBytes,
                target: ref userData);
        }

        public void Unprotect(byte[] encryptedData)
        {
            this.EnsureParameter(encryptedData);
            var decryptedBytes = _encryptor.Decrypt(encryptedData, _key);
            SetBytesToByteArray(
                source: decryptedBytes,
                target: ref encryptedData);
        }

        private static void SetBytesToByteArray(ref byte[] target, byte[] source)
        {
            if (target.Length != source.Length)
                throw new ArgumentException(
                    $"Length of the {nameof(target)} must be equal to the length of {nameof(source)}. {nameof(target)} length was {target.Length}, {nameof(source)} length is {source.Length}");
            for (var i = 0; i < source.Length; i++)
                target[i] = source[i];
        }
    }
}

#endif