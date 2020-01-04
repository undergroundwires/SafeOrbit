using System;
using System.Security.Cryptography;

namespace SafeOrbit.Cryptography.Encryption.Padding.Padders
{
    public class Pkcs7Padder : IPkcs7Padder
    {
        /// <inheritdoc />
        /// <inheritdoc cref="ValidatePaddedBytes" />
        /// <inheritdoc cref="CountPad" />
        public byte[] Unpad(byte[] paddedBytes)
        {
            ValidatePaddedBytes(paddedBytes);
            var totalPaddedBytes = CountPad(paddedBytes);
            var totalUnpaddedBytes = paddedBytes.Length - totalPaddedBytes;
            var unpaddedBytes = new byte[totalUnpaddedBytes];
            Array.Copy(paddedBytes, 0, unpaddedBytes, 0, totalUnpaddedBytes);
            return unpaddedBytes;
        }

        /// <inheritdoc />
        /// <inheritdoc cref="ValidatePaddingLength" />
        /// <exception cref="OverflowException">Padded data is too big</exception>
        /// <exception cref="ArgumentNullException"><paramref name="data" /> is <see langword="null" />.</exception>
        public byte[] Pad(byte[] data, int paddingLengthInBytes)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            ValidatePaddingLength(paddingLengthInBytes);
            var output = new byte[data.Length + paddingLengthInBytes];
            Array.Copy(data, 0, output, 0, data.Length);
            for (var i = data.Length; i < output.Length; i++) output[i] = (byte) paddingLengthInBytes;
            return output;
        }

        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="paddingLengthInBytes" /> is negative, zero or higher than
        ///     256.
        /// </exception>
        private static void ValidatePaddingLength(int paddingLengthInBytes)
        {
            if (paddingLengthInBytes >= 256)
                throw new ArgumentOutOfRangeException(nameof(paddingLengthInBytes),
                    "Cannot be higher than maximum integer in a byte (255)");
            if (paddingLengthInBytes == 0)
                throw new ArgumentOutOfRangeException(nameof(paddingLengthInBytes),
                    "Cannot be zero, pad a whole block instead?");
            if (paddingLengthInBytes < 0)
                throw new ArgumentOutOfRangeException(nameof(paddingLengthInBytes), "Cannot be negative");
        }

        /// <exception cref="CryptographicException">Cannot unpad a single byte.</exception>
        /// <exception cref="ArgumentException"><paramref name="paddedBytes" /> is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="paddedBytes" /> is <see langword="null" />.</exception>
        private static void ValidatePaddedBytes(byte[] paddedBytes)
        {
            if (paddedBytes == null)
                throw new ArgumentNullException(nameof(paddedBytes), $"{nameof(paddedBytes)} can not be null");
            if (paddedBytes.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(paddedBytes));
            if (paddedBytes.Length == 1)
                throw new CryptographicException("Cannot unpad a single byte.");
        }

        /// <summary>
        ///     Get the number of pad bytes present in the block.
        /// </summary>
        /// <exception cref="CryptographicException">Corrupted padded bytes.</exception>
        public static int CountPad(byte[] input)
        {
            var countAsByte = input[input.Length - 1];
            var count = countAsByte;
            if (count < 1 || count > input.Length)
                throw new CryptographicException("Padded bytes are corrupted");
            for (var i = 2; i <= count; i++)
                if (input[input.Length - i] != countAsByte)
                    throw new CryptographicException("Padded bytes are corrupted");
            return count;
        }
    }
}