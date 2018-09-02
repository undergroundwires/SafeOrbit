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

#if (NETCORE)
using System;
using SafeOrbit.Cryptography.Encryption;
using SafeOrbit.Cryptography.Random;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection
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
            if (userData == null) throw new ArgumentNullException(nameof(userData));
            var encryptedBytes = _encryptor.Encrypt(userData, _key);
            SetBytesToByteArray(
                source: encryptedBytes,
                target: ref userData);
        }

        public void Unprotect(byte[] encryptedData)
        {
            if (encryptedData == null) throw new ArgumentNullException(nameof(encryptedData));
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