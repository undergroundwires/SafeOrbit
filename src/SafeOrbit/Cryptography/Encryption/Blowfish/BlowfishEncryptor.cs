
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Exceptions;
using SafeOrbit.Extensions;
using SafeOrbit.Helpers;

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
    /// <seealso cref="IFastEncryptor" />
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

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlowfishEncryptor" /> class with a defined
        ///     <see cref="BlowfishCipherMode" /> and <see cref="ICryptoRandom" />.
        /// </summary>
        /// <param name="chiperMode">The chiper mode.</param>
        /// <param name="random">The random generator to be used for creation of IV's.</param>
        /// <exception cref="UnexpectedEnumValueException{BlowfishCipherMode}">
        ///     <paramref name="chiperMode" /> is not defined in <see cref="BlowfishCipherMode" />.
        /// </exception>
        /// <seealso cref="BlowfishCipherMode" />
        /// <seealso cref="IvSize" />
        /// <seealso cref="ICryptoRandom" />
        public BlowfishEncryptor(BlowfishCipherMode chiperMode, ICryptoRandom random) : base(random)
        {
            if (((int) chiperMode != 0) && ((int) chiperMode != 1))
                throw new UnexpectedEnumValueException<BlowfishCipherMode>(chiperMode);
            CipherMode = chiperMode;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlowfishEncryptor" /> class with a defined
        ///     <see cref="BlowfishCipherMode" />.
        /// </summary>
        /// <param name="chiperMode">The chiper mode.</param>
        /// <exception cref="UnexpectedEnumValueException{BlowfishCipherMode}">
        ///     <paramref name="chiperMode" /> is not defined in
        ///     <see cref="BlowfishCipherMode" />.
        /// </exception>
        /// <seealso cref="BlowfishCipherMode" />
        /// <seealso cref="IvSize" />
        public BlowfishEncryptor(BlowfishCipherMode chiperMode) : this(chiperMode, FastRandom.StaticInstance)
        {
        }

        public BlowfishCipherMode CipherMode { get; } = DefaultCipherMode;
        public override int MinKeySize { get; } = 32;
        public override int MaxKeySize { get; } = 448;
        public override int BlockSize { get; } = 64;
        public override int IvSize => this.CipherMode == BlowfishCipherMode.Ecb ? 0 : 8;

        /// <summary>
        ///     Encrypts the specified input using <see cref="CipherMode" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="input" /> is <see langword="null" /> or empty.</p>
        ///     <p><paramref name="key" /> is <see langword="null" /> or empty.</p>
        /// </exception>
        /// <exception cref="KeySizeException">
        ///     Length of the <paramref name="key" /> must be between <see cref="MinKeySize" /> and
        ///     <see cref="MaxKeySize" /> values.
        /// </exception>
        /// <exception cref="UnexpectedEnumValueException{TEnum}"><see cref="CipherMode" /> is not defined or supported.</exception>
        /// <seealso cref="EncryptAsync" />
        public byte[] Encrypt(byte[] input, byte[] key) => AsyncHelper.RunSync(() => EncryptAsync(input, key));

        /// <summary>
        ///     Decrypts the specified input using <see cref="CipherMode" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="input" /> is <see langword="null" /> or empty.</p>
        ///     <p><paramref name="key" /> is <see langword="null" /> or empty.</p>
        /// </exception>
        /// <exception cref="KeySizeException">
        ///     Length of the <paramref name="key" /> must be between <see cref="MinKeySize" /> and
        ///     <see cref="MaxKeySize" /> values.
        /// </exception>
        /// <exception cref="UnexpectedEnumValueException{TEnum}"><see cref="CipherMode" /> is not defined or supported.</exception>
        /// <seealso cref="DecryptAsync" />
        public byte[] Decrypt(byte[] input, byte[] key) => AsyncHelper.RunSync(() => DecryptAsync(input, key));

        /// <summary>
        ///     Encrypts the specified input using <see cref="CipherMode" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="input" /> is <see langword="null" /> or empty.</p>
        ///     <p><paramref name="key" /> is <see langword="null" /> or empty.</p>
        /// </exception>
        /// <exception cref="KeySizeException">
        ///     Length of the <paramref name="key" /> must be between <see cref="MinKeySize" /> and
        ///     <see cref="MaxKeySize" /> values.
        /// </exception>
        /// <exception cref="UnexpectedEnumValueException{TEnum}"><see cref="CipherMode" /> is not defined or supported.</exception>
        /// <seealso cref="Encrypt" />
        public Task<byte[]> EncryptAsync(byte[] input, byte[] key)
        {
            EnsureParameters(input, key);
            switch (CipherMode)
            {
                case BlowfishCipherMode.Ecb:
                    return InternalCryptEcbAsync(input, key, true);
                case BlowfishCipherMode.Cbc:
                    return InternalCryptCbcAsync(input, key, true);
                default:
                    throw new UnexpectedEnumValueException<BlowfishCipherMode>(CipherMode);
            }
        }

        /// <summary>
        ///     Decrypts the specified input using <see cref="CipherMode" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="input" /> is <see langword="null" /> or empty.</p>
        ///     <p><paramref name="key" /> is <see langword="null" /> or empty.</p>
        /// </exception>
        /// <exception cref="KeySizeException">
        ///     Length of the <paramref name="key" /> must be between <see cref="MinKeySize" /> and
        ///     <see cref="MaxKeySize" /> values.
        /// </exception>
        /// <exception cref="UnexpectedEnumValueException{TEnum}"><see cref="CipherMode" /> is not defined or supported.</exception>
        /// <seealso cref="Decrypt" />
        public Task<byte[]> DecryptAsync(byte[] input, byte[] key)
        {
            EnsureParameters(input, key);
            switch (CipherMode)
            {
                case BlowfishCipherMode.Ecb:
                    return InternalCryptEcbAsync(input, key, false);
                case BlowfishCipherMode.Cbc:
                    return InternalCryptCbcAsync(input, key, false);
                default:
                    throw new UnexpectedEnumValueException<BlowfishCipherMode>(CipherMode);
            }
        }

        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="input" /> is <see langword="null" /> or empty.</p>
        ///     <p><paramref name="key" /> is <see langword="null" /> or empty.</p>
        /// </exception>
        /// <exception cref="KeySizeException">
        ///     Length of the <paramref name="key" /> must be between <see cref="MinKeySize" /> and
        ///     <see cref="MaxKeySize" /> values.
        /// </exception>
        private void EnsureParameters(byte[] input, byte[] key)
        {
            if ((input == null) || !input.Any()) throw new ArgumentNullException(nameof(input));
            if ((key == null) || !key.Any()) throw new ArgumentNullException(nameof(key));
            EnsureKeyParameter(key);
        }


        private async Task<byte[]> InternalCryptEcbAsync(byte[] input, byte[] key, bool forEncryption)
        {
            using (var ms = new MemoryStream())
            {
                using (var blowfish = new BlowfishEcb(key, forEncryption))
                {
                    using (var cs = new CryptoStream(ms, blowfish, CryptoStreamMode.Write))
                    {
                        await cs.WriteAsync(input, 0, input.Length);
                    }
                }
                return ms.ToArray();
            }
        }

        private Task<byte[]> InternalCryptCbcAsync(byte[] input, byte[] key, bool forEncryption)
        {
            return forEncryption ? InternalEncryptCbcAsync(input, key) : InternalDecryptCbcAsync(input, key);
        }

        private async Task<byte[]> InternalDecryptCbcAsync(byte[] input, byte[] key)
        {
            var iv = new byte[IvSize];
            var encryptedContent = new byte[input.Length - IvSize];
            Buffer.BlockCopy(input, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(input, iv.Length, encryptedContent, 0, encryptedContent.Length);
            using (var ms = new MemoryStream())
            {
                using (var blowfish = new BlowfishCbc(key, iv, false))
                {
                    using (var cs = new CryptoStream(ms, blowfish, CryptoStreamMode.Write))
                    {
                        await cs.WriteAsync(encryptedContent, 0, encryptedContent.Length);
                    }
                }
                return ms.ToArray();
            }
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
                    using (var cs = new CryptoStream(ms, blowfish, CryptoStreamMode.Write))
                    {
                        await cs.WriteAsync(input, 0, input.Length);
                    }
                }
                encryptedContent = ms.ToArray();
            }
            //Create new byte array that should contain both unencrypted iv and encrypted data
            var result = iv.Combine(encryptedContent);
            return result;
        }
    }
}