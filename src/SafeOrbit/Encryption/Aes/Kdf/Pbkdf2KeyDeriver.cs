using System.Security.Cryptography;

namespace SafeOrbit.Encryption.Kdf
{
    /// <summary>
    ///     An wrapper for <see cref="Rfc2898DeriveBytes" />
    /// </summary>
    /// <seealso cref="IKeyDerivationFunction" />
    public class Pbkdf2KeyDeriver : IKeyDerivationFunction
    {
        /// <summary>
        ///     The difficulty of a brute force attack increases with the number of iterations. A practical limit on the iteration
        ///     count is the unwillingness of users to tolerate a perceptible delay in logging in to a computer or seeing a
        ///     decrypted message. The use of salt prevents the attackers from precomputing a dictionary of derived keys.
        /// </summary>
        private const int DefaultTotalIterations = 100;

        /// <summary>
        /// Uses key derivation function to strengthen the given <paramref name="key" />.
        /// </summary>
        /// <param name="key">The master password from which a derived key is generated,</param>
        /// <param name="salt">The cryptographic salt.</param>
        /// <param name="length">The length of the derived bytes.</param>
        /// <returns>The derived key</returns>
        public byte[] Derive(byte[] key, byte[] salt, int length)
        {
            var deriveBytes = new Rfc2898DeriveBytes(key, salt, TotalIterations);
            var result = deriveBytes.GetBytes(length);
            return result;
        }

        public int TotalIterations { get; set; } = DefaultTotalIterations;
    }
}