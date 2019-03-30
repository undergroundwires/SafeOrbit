namespace SafeOrbit.Cryptography.Random
{
    /// <summary>
    ///     Abstracts a cryptographically secure random generator.
    /// </summary>
    public interface ICryptoRandom
    {
        /// <summary>
        ///     Gets an array of bytes with a cryptographically strong sequence of random values for the specified length.
        /// </summary>
        /// <param name="length">The length of the byte array.</param>
        /// <returns>Cryptographically strong sequence of random values.</returns>
        byte[] GetBytes(int length);

        /// <summary>
        ///     Gets a cryptographically random int.
        /// </summary>
        /// <returns>Cryptographically random int</returns>
        int GetInt();

        /// <summary>
        ///     Gets a cryptographically random int between <paramref name="min" /> a nd <paramref name="max" />.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>Cryptographically random int</returns>
        int GetInt(int min, int max);

        /// <summary>
        ///     Gets a value of cryptographically strong true/false condition.
        /// </summary>
        /// <returns>Cryptographically random <c>true</c> or <c>false</c> value.</returns>
        bool GetBool();
    }
}