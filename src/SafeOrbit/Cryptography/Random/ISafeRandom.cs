namespace SafeOrbit.Cryptography.Random
{
    /// <summary>
    ///     Abstracts cryptographically secure random generator.
    /// </summary>
    /// <seealso cref="ICryptoRandom" />
    public interface ISafeRandom : ICryptoRandom
    {
#if NETFRAMEWORK

        /// <summary>
        /// Gets the non zero bytes.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>System.Byte[].</returns>
        byte[] GetNonZeroBytes(int length);
#endif
    }
}