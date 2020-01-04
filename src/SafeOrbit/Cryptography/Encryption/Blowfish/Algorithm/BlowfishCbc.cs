using System;
using System.Collections.Generic;
using SafeOrbit.Exceptions;

namespace SafeOrbit.Cryptography.Encryption
{
    /// <inheritdoc />
    /// <summary>
    ///     Blowfish implementation with <see cref="F:System.Security.Cryptography.CipherMode.CBC" />
    /// </summary>
    /// <seealso cref="T:System.Security.Cryptography.ICryptoTransform" />
    /// <seealso cref="F:System.Security.Cryptography.CipherMode.CBC" />
    /// <seealso cref="T:SafeOrbit.Cryptography.Encryption.BlowfishEcb" />
    /// <seealso cref="T:SafeOrbit.Cryptography.Encryption.BlowfishEncryptor" />
    public class BlowfishCbc : BlowfishEcb
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:SafeOrbit.Cryptography.Encryption.BlowfishCbc" /> class.
        /// </summary>
        /// <param name="cipherKey">The cipher key.</param>
        /// <param name="iv">The iv.</param>
        /// <param name="forEncryption">if set to <c>true</c> [for encryption].</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="cipherKey" /> is <see langword="null" />.</exception>
        /// &gt;
        /// <exception cref="T:SafeOrbit.Exceptions.KeySizeException">
        ///     <paramref name="cipherKey" /> is must be between 32 bits (4
        ///     bytes) and 488 bits (56) size.
        /// </exception>
        /// &gt;
        /// <exception cref="T:System.ArgumentNullException"><paramref name="iv" /> is <see langword="null" />.</exception>
        /// &gt;
        /// <exception cref="T:SafeOrbit.Exceptions.DataLengthException">
        ///     <paramref name="iv" />  size must be
        ///     <see cref="BlowfishEcb.InputBlockSize" />.
        /// </exception>
        public BlowfishCbc(byte[] cipherKey, byte[] iv, bool forEncryption) : base(cipherKey, forEncryption)
        {
            if (iv == null) throw new ArgumentNullException(nameof(iv));
            if (iv.Length != InputBlockSize)
                throw new DataLengthException(nameof(iv),
                    $"{iv} size must be {InputBlockSize} bytes ({InputBlockSize * 8} bits)");
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
            var resultLen = inputCount % outputLength == 0
                ? inputCount
                : inputCount + outputLength - inputCount % outputLength;
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
        private static void XorBlock(ref byte[] block, IReadOnlyList<byte> iv)
        {
            for (var i = 0; i < block.Length; i++)
                block[i] ^= iv[i];
        }
    }
}