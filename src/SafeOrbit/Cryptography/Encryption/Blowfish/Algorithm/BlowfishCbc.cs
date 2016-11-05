
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

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
using System.Security.Cryptography;
using SafeOrbit.Exceptions;

namespace SafeOrbit.Cryptography.Encryption
{
    /// <summary>
    ///     Default blowfish implementation with CBC cipher mode.
    /// </summary>
    /// <remarks>
    ///     CBC mode encrypts each block of data in succession so that any changes in the data will result in a completly
    ///     different ciphertext. Also, an IV is used so that encrypting the same data with the same key will result in a
    ///     different ciphertext. CBC mode is the most popular mode of operation.
    ///     Read more : http://en.wikipedia.org/wiki/Block_cipher_modes_of_operation
    /// </remarks>
    /// <exception cref="iv"></exception>
    /// <seealso cref="ICryptoTransform" />
    public class BlowfishCbc : BlowfishEcb
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlowfishCbc"/> class.
        /// </summary>
        /// <param name="cipherKey">The cipher key.</param>
        /// <param name="iv">The iv.</param>
        /// <param name="forEncryption">if set to <c>true</c> [for encryption].</param>
        /// <exception cref="ArgumentNullException"><paramref name="cipherKey" /> is <see langword="null" />.</exception>>
        /// <exception cref="KeySizeException"><paramref name="cipherKey" /> is must be between 32 bits (4 bytes) and 488 bits (56) size.</exception>>
        /// <exception cref="ArgumentNullException"><paramref name="iv" /> is <see langword="null" />.</exception>>
        public BlowfishCbc(byte[] cipherKey, byte[] iv, bool forEncryption) : base(cipherKey, forEncryption)
        {
            if (iv == null) throw new ArgumentNullException(nameof(iv));
            if (iv.Length != InputBlockSize) throw new DataLengthException(nameof(iv), $"{iv} size must be {InputBlockSize} bytes ({InputBlockSize * 8} bytes)");
            Iv = iv;
        }

        public byte[] Iv { get; }

        public override int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer,
            int outputOffset)
        {
            var processedBytes = 0;
            var blockSize = OutputBlockSize;
            for (var i = 0; i < inputCount; i += blockSize) //each block
            {
                var block = new byte[blockSize];
                Buffer.BlockCopy(inputBuffer, inputOffset + i, block, 0, blockSize);
                if (ForEncryption)
                {
                    XorBlock(ref block, Iv);
                    BlockEncrypt(ref block);
                }
                else
                {
                    BlockDecrypt(ref block);
                    XorBlock(ref block, Iv);
                }
                Buffer.BlockCopy(block, 0, outputBuffer, outputOffset + i, blockSize);
                processedBytes += blockSize;
            }
            return processedBytes;
        }

        public override byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            var outputLength = OutputBlockSize;
            var inputLength = InputBlockSize;
            var resultLen = inputCount% outputLength == 0
                ? inputCount
                : inputCount + outputLength - inputCount% outputLength;
            var result = new byte[resultLen];
            for (var i = 0; i < resultLen; i += inputLength) //each block
            {
                var block = new byte[outputLength];
                Buffer.BlockCopy(inputBuffer, inputOffset + i, block, 0, inputLength);
                if (ForEncryption)
                {
                    XorBlock(ref block, Iv);
                    BlockEncrypt(ref block);
                }
                else
                {
                    BlockDecrypt(ref block);
                    XorBlock(ref block, Iv);
                }
                Buffer.BlockCopy(block, 0, result, i, block.Length);
            }
            return result;
        }

        /// <summary>
        ///     XoR encrypts two 8 bit blocks
        /// </summary>
        /// <param name="block">8 bit block 1</param>
        /// <param name="iv">8 bit block 2</param>
        private static void XorBlock(ref byte[] block, byte[] iv)
        {
            for (var i = 0; i < block.Length; i++)
                block[i] ^= iv[i];
        }
    }
}