
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