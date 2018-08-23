
/*
MIT License

Copyright (c) 2018 Erkin Ekici - undergroundwires@safeorb.it

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