namespace SafeOrbit.Cryptography.Encryption.Kdf
{
    /// <summary>
    ///     <p></p>An interface defining a key deriver function.
    ///     <p>More: https://en.wikipedia.org/wiki/Key_derivation_function </p>
    /// </summary>
    /// <remarks>
    ///     <p>
    ///         Key derivation functions are in applications to derive keys from secret passwords or passphrases,
    ///         which typically do not have the desired properties to be used directly as cryptographic keys. In such
    ///         applications, it is generally recommended that the key derivation function be made deliberately slow so as to
    ///         frustrate brute-force attack or dictionary attack on the password or passphrase input value.
    ///     </p>
    ///     <p>DK = KDF(Key,Salt,Iterations)</p>
    /// </remarks>
    public interface IKeyDerivationFunction
    {
        /// <summary>
        ///     <p>Gets the total iterations. </p>
        ///     <p>
        ///         The difficulty of a brute force attack increases with the number of iterations. A practical limit on the
        ///         iteration count is the unwillingness of users to tolerate a perceptible delay in logging in to a computer or
        ///         seeing a decrypted message. The use of salt prevents the attackers from precomputing a dictionary of derived
        ///         keys.
        ///     </p>
        /// </summary>
        /// <value>The total iterations.</value>
        int TotalIterations { get; set; }

        /// <summary>
        ///     Uses key derivation function to strengthen the given <paramref name="key" />.
        /// </summary>
        /// <param name="key">The original key or password</param>
        /// <param name="salt">The cryptographic salt</param>
        /// <param name="length">The length of the derived bytes.</param>
        /// <returns>The derived key</returns>
        byte[] Derive(byte[] key, byte[] salt, int length);
    }
}