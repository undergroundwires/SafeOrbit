namespace SafeOrbit.Cryptography.Encryption.Padding
{

    public enum PaddingMode
    {
        /// <summary>
        ///     No padding is done.
        /// </summary>
        None = 1,
        /// <summary>
        ///     The PKCS #7 padding string consists of a sequence of bytes, each of which is equal to the total number of padding bytes added.
        /// </summary>
        PKCS7 = 2
    }
}
