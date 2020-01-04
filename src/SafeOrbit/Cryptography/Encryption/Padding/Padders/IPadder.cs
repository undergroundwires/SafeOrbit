namespace SafeOrbit.Cryptography.Encryption.Padding.Padders
{
    public interface IPadder
    {
        /// <summary>
        ///     Removes the PKCS7 padding of an array.
        /// </summary>
        /// <param name="paddedBytes">The padded array.</param>
        /// <returns>The unpadded array.</returns>
        byte[] Unpad(byte[] paddedBytes);

        /// <summary>
        ///     Fill up an array with PKCS7 padding.
        /// </summary>
        /// <param name="data">The source array.</param>
        /// <param name="paddingLengthInBytes">Additional padded bytes to add</param>
        /// <returns>The padded array.</returns>
        byte[] Pad(byte[] data, int paddingLengthInBytes);
    }
}