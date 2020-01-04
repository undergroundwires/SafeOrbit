using System;
using System.Security.Cryptography;
using SafeOrbit.Cryptography.Encryption.Blowfish.Algorithm;
using SafeOrbit.Cryptography.Encryption.Blowfish.Algorithm.SubkeyArrays;
using SafeOrbit.Exceptions;
using SafeOrbit.Extensions;

namespace SafeOrbit.Cryptography.Encryption
{
    /// <summary>
    ///     Blowfish implementation with <see cref="CipherMode.ECB" />.
    ///     <b>ECB</b> mode encrypts each block of data with the same key, so patterns in a large set of data will be visible.
    /// </summary>
    /// <seealso cref="ICryptoTransform" />
    /// <seealso cref="CipherMode.ECB" />
    /// <seealso cref="BlowfishCbc" />
    /// <seealso cref="BlowfishEncryptor" />
    public class BlowfishEcb : ICryptoTransform
    {
        /// <summary>
        ///     Standard is 16
        /// </summary>
        /// <remarks>To increase the number of rounds, bf_P needs to be equal to the number of rounds. Use digits of PI.</remarks>
        private const int Rounds = 16;


        private readonly byte[] _key;
        private uint[] _pArray;
        private uint[][] _sArray;

        //HALF-BLOCKS
        private uint _xlPar;
        private uint _xrPar;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlowfishEcb" /> class.
        /// </summary>
        /// <param name="cipherKey">The cipher key.</param>
        /// <param name="forEncryption">if set to <c>true</c> then the blocks will be encrypted; otherwise: they'll be decrypted.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cipherKey" /> is <see langword="null" />.</exception>
        /// >
        /// <exception cref="KeySizeException">
        ///     <paramref name="cipherKey" /> is must be between 32 bits (4 bytes) and 488 bits (56)
        ///     size.
        /// </exception>
        /// >
        public BlowfishEcb(byte[] cipherKey, bool forEncryption)
        {
            if (cipherKey == null) throw new ArgumentNullException(nameof(cipherKey));
            if (cipherKey.Length < 4 /* 32 bits */ || cipherKey.Length > 56 /* 488 bits */)
                throw new KeySizeException(minSize: 32, maxSize: 488, actual: cipherKey.Length);
            ForEncryption = forEncryption;
            _key = SetKey(cipherKey);
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is configured for encryption or decryption.
        /// </summary>
        /// <value><c>true</c> if for encryption;  <c>false</c> if for decryption.</value>
        public bool ForEncryption { get; }

        public void Dispose()
        {
            if (_key != null)
                Array.Clear(_key, 0, _key.Length);
            if (_sArray != null)
            {
                foreach (var array in _sArray)
                {
                    if (array == null) continue;
                    Array.Clear(array, 0, array.Length);
                }

                Array.Clear(_sArray, 0, _sArray.Length);
            }

            if (_pArray != null)
                Array.Clear(_pArray, 0, _pArray.Length);
        }

        /// <summary>
        ///     Encrypt/decrypts the block.
        /// </summary>
        /// <param name="inputBuffer">The input buffer.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <param name="inputCount">The input count.</param>
        /// <param name="outputBuffer">The output buffer.</param>
        /// <param name="outputOffset">The output offset.</param>
        /// <returns>The length of the processed bytes.</returns>
        public virtual int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer,
            int outputOffset)
        {
            if (inputCount == 0) return 0;
            var block = new byte[inputCount];
            Buffer.BlockCopy(inputBuffer, inputOffset, block, 0, inputCount);
            if (ForEncryption)
                BlockEncrypt(ref block);
            else
                BlockDecrypt(ref block);
            Buffer.BlockCopy(block, 0, outputBuffer, outputOffset, inputCount);
            return inputCount;
        }

        /// <summary>
        ///     Transforms the final block.
        /// </summary>
        /// <param name="inputBuffer">The input buffer.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <param name="inputCount">The input count.</param>
        /// <returns>System.Byte[].</returns>
        public virtual byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
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
                    BlockEncrypt(ref block);
                else
                    BlockDecrypt(ref block);
                Buffer.BlockCopy(block, 0, result, i, block.Length);
            }

            return result;
        }

        public int InputBlockSize { get; } = 8;
        public int OutputBlockSize { get; } = 8;
        public bool CanTransformMultipleBlocks { get; } = false;
        public bool CanReuseTransform { get; } = true;


        /// <summary>
        ///     Key the Blowfish cipher.
        /// </summary>
        /// <param name="cipherKey">Block cipher key (1-448 bits)</param>
        private byte[] SetKey(byte[] cipherKey)
        {
            _pArray = PBoxFactory.GetPArray();
            _sArray = SBoxFactory.GetSArray();
            var key = cipherKey.CopyToNewArray();
            var j = 0;
            for (var i = 0; i < 18; i++)
            {
                var d = (uint) (((key[j % cipherKey.Length] * 256 + key[(j + 1) % cipherKey.Length]) * 256 +
                                 key[(j + 2) % cipherKey.Length]) * 256 + key[(j + 3) % cipherKey.Length]);
                _pArray[i] ^= d;
                j = (j + 4) % cipherKey.Length;
            }

            _xlPar = 0;
            _xrPar = 0;
            for (var i = 0; i < 18; i += 2)
            {
                Encipher();
                _pArray[i] = _xlPar;
                _pArray[i + 1] = _xrPar;
            }

            for (var i = 0; i < 256; i += 2)
            {
                Encipher();
                _sArray[0][i] = _xlPar;
                _sArray[0][i + 1] = _xrPar;
            }

            for (var i = 0; i < 256; i += 2)
            {
                Encipher();
                _sArray[1][i] = _xlPar;
                _sArray[1][i + 1] = _xrPar;
            }

            for (var i = 0; i < 256; i += 2)
            {
                Encipher();
                _sArray[2][i] = _xlPar;
                _sArray[2][i + 1] = _xrPar;
            }

            for (var i = 0; i < 256; i += 2)
            {
                Encipher();
                _sArray[3][i] = _xlPar;
                _sArray[3][i + 1] = _xrPar;
            }

            return key;
        }

        /// <summary>
        ///     Encrypts a 64 bit block
        /// </summary>
        /// <param name="block">The 64 bit block to encrypt</param>
        protected void BlockEncrypt(ref byte[] block)
        {
            SetBlock(block);
            Encipher();
            GetBlock(ref block);
        }

        /// <summary>
        ///     Decrypts a 64 bit block
        /// </summary>
        /// <param name="block">The 64 bit block to decrypt</param>
        protected void BlockDecrypt(ref byte[] block)
        {
            SetBlock(block);
            Decipher();
            GetBlock(ref block);
        }

        /// <summary>
        ///     Splits the block into the two uint values
        /// </summary>
        /// <param name="block">the 64 bit block to setup</param>
        private void SetBlock(byte[] block)
        {
            var block1 = new byte[4];
            var block2 = new byte[4];
            // Split the block
            Buffer.BlockCopy(block, 0, block1, 0, 4);
            Buffer.BlockCopy(block, 4, block2, 0, 4);
            // ToUInt32 requires the bytes in reverse order
            Array.Reverse(block1);
            Array.Reverse(block2);
            _xlPar = BitConverter.ToUInt32(block1, 0);
            _xrPar = BitConverter.ToUInt32(block2, 0);
        }

        /// <summary>
        ///     Converts the two uint values into a 64 bit block
        /// </summary>
        /// <param name="block">64 bit buffer to receive the block</param>
        private void GetBlock(ref byte[] block)
        {
            var block1 = BitConverter.GetBytes(_xlPar);
            var block2 = BitConverter.GetBytes(_xrPar);
            // GetBytes returns the bytes in reverse order
            Array.Reverse(block1);
            Array.Reverse(block2);
            // Join the block
            Buffer.BlockCopy(block1, 0, block, 0, 4);
            Buffer.BlockCopy(block2, 0, block, 4, 4);
        }

        /// <summary>
        ///     Runs the blowfish algorithm (standard 16 rounds)
        /// </summary>
        private void Encipher()
        {
            _xlPar ^= _pArray[0];
            for (uint i = 0; i < Rounds; i += 2)
            {
                _xrPar = Round(_xrPar, _xlPar, i + 1);
                _xlPar = Round(_xlPar, _xrPar, i + 2);
            }

            _xrPar ^= _pArray[17];
            // Swap the blocks
            var swap = _xlPar;
            _xlPar = _xrPar;
            _xrPar = swap;
        }

        /// <summary>
        ///     Runs the blowfish algorithm in reverse (standard 16 rounds)
        /// </summary>
        private void Decipher()
        {
            _xlPar ^= _pArray[17];
            for (uint i = 16; i > 0; i -= 2)
            {
                _xrPar = Round(_xrPar, _xlPar, i);
                _xlPar = Round(_xlPar, _xrPar, i - 1);
            }

            _xrPar ^= _pArray[0];
            // Swap the blocks
            var swap = _xlPar;
            _xlPar = _xrPar;
            _xrPar = swap;
        }

        /// <summary>
        ///     One round of the blowfish algorithm
        /// </summary>
        private uint Round(uint a, uint b, uint n)
        {
            var x1 = (_sArray[0][b.GetFirstByte()] + _sArray[1][b.GetSecondByte()]) ^ _sArray[2][b.GetThirdByte()];
            var x2 = x1 + _sArray[3][b.GetFourthByte()];
            var x3 = x2 ^ _pArray[n];
            return x3 ^ a;
        }
    }
}