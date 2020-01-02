using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Exceptions;
using SafeOrbit.Extensions;

namespace SafeOrbit.Cryptography.Encryption
{
    /// <summary>
    ///     <p>
    ///         Block cipher: 64-bit block
    ///         Variable key length: 32 bits to 448 bits
    ///         Much faster than DES and IDEA.
    ///     </p>
    /// </summary>
    /// <remarks>
    ///     <p>
    ///         Blowfish is a fast block cipher, except when changing keys. Each new key requires pre-processing equivalent to
    ///         encrypting about 4 kilobytes of text, which is very slow compared to other block ciphers. This prevents its use
    ///         in certain applications, but is not a problem in others.
    ///         More : https://en.wikipedia.org/wiki/Blowfish_(cipher)#Blowfish_in_practice
    ///     </p>
    ///     <p>https://www.schneier.com/academic/blowfish/</p>
    /// </remarks>
    /// <seealso cref="T:SafeOrbit.Cryptography.Encryption.IFastEncryptor" />
    /// <inheritdoc cref="EncryptorBase" />
    /// <inheritdoc cref="IFastEncryptor" />
    public class BlowfishEncryptor : EncryptorBase, IFastEncryptor
    {
        public const BlowfishCipherMode DefaultCipherMode = BlowfishCipherMode.Cbc;
        public static IFastEncryptor StaticInstance = new BlowfishEncryptor(BlowfishCipherMode.Cbc);

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlowfishEncryptor" /> class.
        /// </summary>
        public BlowfishEncryptor() : base(FastRandom.StaticInstance)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlowfishEncryptor" /> class with a defined
        ///     <see cref="ICryptoRandom" />.
        /// </summary>
        /// <param name="random">The random generator to be used for creation of IV's.</param>
        public BlowfishEncryptor(ICryptoRandom random) : base(random)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:SafeOrbit.Cryptography.Encryption.BlowfishEncryptor" /> class with a defined
        ///     <see cref="T:SafeOrbit.Cryptography.Encryption.BlowfishCipherMode" />.
        /// </summary>
        /// <param name="cipherMode">The cipher mode.</param>
        /// <exception cref="UnexpectedEnumValueException{BlowfishCipherMode}">
        ///     <paramref name="cipherMode" /> is not defined in <see cref="BlowfishCipherMode" />.
        /// </exception>
        /// <seealso cref="BlowfishCipherMode" />
        /// <seealso cref="IvSizeInBits" />
        public BlowfishEncryptor(BlowfishCipherMode cipherMode, ICryptoRandom random) : base(random)
        {
            if (((int) cipherMode != 0) && ((int) cipherMode != 1))
                throw new UnexpectedEnumValueException<BlowfishCipherMode>(cipherMode);
            CipherMode = cipherMode;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlowfishEncryptor" /> class with a defined
        ///     <see cref="BlowfishCipherMode" />.
        /// </summary>
        /// <param name="cipherMode">The cipher mode.</param>
        /// <exception cref="UnexpectedEnumValueException{BlowfishCipherMode}">
        ///     <paramref name="cipherMode" /> is not defined in <see cref="BlowfishCipherMode" />.
        /// </exception>
        /// <seealso cref="BlowfishCipherMode" />
        /// <seealso cref="IvSizeInBits" />
        public BlowfishEncryptor(BlowfishCipherMode cipherMode) : this(cipherMode, FastRandom.StaticInstance)
        {
        }

        public BlowfishCipherMode CipherMode { get; } = DefaultCipherMode;
        public override int MinKeySizeInBits { get; } = 32;
        public override int MaxKeySizeInBits { get; } = 448;
        public override int BlockSizeInBits { get; } = 64;
        public override int IvSizeInBits => this.CipherMode == BlowfishCipherMode.Ecb ? 0 : 8;

        /// <inheritdoc cref="EncryptAsync" />
        public byte[] Encrypt(byte[] input, byte[] key) => TaskContext.RunSync(() => EncryptAsync(input, key));

        /// <inheritdoc cref="DecryptAsync" />
        public byte[] Decrypt(byte[] input, byte[] key) => TaskContext.RunSync(() => DecryptAsync(input, key));

        /// <inheritdoc />
        /// <summary>
        ///     Encrypts the specified input using <see cref="CipherMode" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="input" /> is <see langword="null" /> or empty.</p>
        ///     <p><paramref name="key" /> is <see langword="null" /> or empty.</p>
        /// </exception>
        /// <exception cref="KeySizeException">
        ///     Length of the <paramref name="key" /> must be between <see cref="MinKeySizeInbits" /> and <see cref="MaxKeySizeInBits" /> values.
        /// </exception>
        /// <exception cref="DataLengthException"> Length of the <paramref name="input" /> is empty </exception>
        public Task<byte[]> EncryptAsync(byte[] input, byte[] key)
        {
            EnsureParameters(input, key);
            return CipherMode switch
            {
                BlowfishCipherMode.Ecb => InternalCryptEcbAsync(input, key, true),
                BlowfishCipherMode.Cbc => InternalCryptCbcAsync(input, key, true),
                _ => throw new UnexpectedEnumValueException<BlowfishCipherMode>(CipherMode)
            };
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="input" /> is <see langword="null" /> or empty.</p>
        ///     <p><paramref name="key" /> is <see langword="null" /> or empty.</p>
        /// </exception>
        /// <exception cref=KeySizeException">
        ///     Length of the <paramref name="key" /> must be between <see cref="MinKeySizeInBits" /> and <see cref="MaxKeySizeInBits" /> values.
        /// </exception>
        /// <exception cref="DataLengthException"> Length of the <paramref name="input" /> is empty </exception>
        public Task<byte[]> DecryptAsync(byte[] input, byte[] key)
        {
            EnsureParameters(input, key);
            return CipherMode switch
            {
                BlowfishCipherMode.Ecb => InternalCryptEcbAsync(input, key, false),
                BlowfishCipherMode.Cbc => InternalCryptCbcAsync(input, key, false),
                _ => throw new UnexpectedEnumValueException<BlowfishCipherMode>(CipherMode)
            };
        }

        /// <exception cref="ArgumentNullException"><paramref name="data" /> is null</exception>
        /// <exception cref="KeySizeException">
        ///     Length of the <paramref name="key" /> must be between <see cref="MinKeySizeInBits" /> and
        ///     <see cref="MaxKeySizeInBits" /> values.
        /// </exception>
        /// <exception cref="DataLengthException"> Length of the <paramref name="data" /> is empty </exception>
        private void EnsureParameters(byte[] data, byte[] key)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            ValidateKey(key);
            if (data.Length == 0)
                throw new DataLengthException(nameof(data), "Data is empty");
        }
        
        private static async Task<byte[]> InternalCryptEcbAsync(byte[] input, byte[] key, bool forEncryption)
        {
            using var ms = new MemoryStream();
            using (var blowfish = new BlowfishEcb(key, forEncryption))
            {
                using var cs = new CryptoStream(ms, blowfish, CryptoStreamMode.Write);
                await cs.WriteAsync(input, 0, input.Length).ConfigureAwait(false);
            }
            return ms.ToArray();
        }

        private Task<byte[]> InternalCryptCbcAsync(byte[] input, byte[] key, bool forEncryption)
        {
            return forEncryption ? InternalEncryptCbcAsync(input, key) : InternalDecryptCbcAsync(input, key);
        }

        private async Task<byte[]> InternalDecryptCbcAsync(byte[] input, byte[] key)
        {
            var iv = new byte[IvSizeInBits];
            var encryptedContent = new byte[input.Length - IvSizeInBits];
            Buffer.BlockCopy(input, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(input, iv.Length, encryptedContent, 0, encryptedContent.Length);
            using var ms = new MemoryStream();
            using (var blowfish = new BlowfishCbc(key, iv, false))
            {
                using var cs = new CryptoStream(ms, blowfish, CryptoStreamMode.Write);
                await cs.WriteAsync(encryptedContent, 0, encryptedContent.Length).ConfigureAwait(false);
            }
            return ms.ToArray();
        }

        private async Task<byte[]> InternalEncryptCbcAsync(byte[] input, byte[] key)
        {
            byte[] iv = null;
            byte[] encryptedContent = null;
            using (var ms = new MemoryStream())
            {
                iv = GetIv();
                using (var blowfish = new BlowfishCbc(key, iv, true))
                {
                    using var cs = new CryptoStream(ms, blowfish, CryptoStreamMode.Write);
                    await cs.WriteAsync(input, 0, input.Length).ConfigureAwait(false);
                }
                encryptedContent = ms.ToArray();
            }
            //Create new byte array that should contain both unencrypted iv and encrypted data
            var result = iv.Combine(encryptedContent);
            return result;
        }
        
    }
}