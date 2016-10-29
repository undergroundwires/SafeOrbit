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

using System;
using System.Linq;

namespace SafeOrbit.Extensions
{
    public static class ByteArrayExtensions
    {
        /// <summary>
        ///     Appends the byte array after the existing one
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="byteArray" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="byteArrays" /> is <see langword="null" />.</exception>
        /// <exception cref="OverflowException">The sum of byte array length larger than <see cref="F:System.Int32.MaxValue" />.</exception>
        public static byte[] Combine(this byte[] byteArray, params byte[][] byteArrays)
        {
            if (byteArray == null) throw new ArgumentNullException(nameof(byteArray));
            if ((byteArrays == null) || byteArrays.Any(b => b == null))
                throw new ArgumentNullException(nameof(byteArrays));
            var newArrayLength = byteArrays.Sum(b => b.Length) + byteArray.Length;
            var combinedByte = new byte[newArrayLength];
            var offset = 0;
            //Copy the first byte
            Buffer.BlockCopy(byteArray, 0, combinedByte, offset, byteArray.Length);
            offset += byteArray.Length;
            //Copy the parameters
            foreach (var array in byteArrays)
            {
                Buffer.BlockCopy(array, 0, combinedByte, offset, array.Length);
                offset += array.Length;
            }
            return combinedByte;
        }

        /// <summary>
        ///     Compares two byte arrays without revealing them all in the memory to protect against timing-attacks.
        /// </summary>
        /// <remarks>
        ///     <p>
        ///         Comparing the hashes in "length-constant" time ensures that an attacker cannot extract the hash of a password
        ///         in an
        ///         on-line system using a timing attack, then crack it off-line. The standard way to check if two sequences of
        ///         bytes(strings) are the same is to compare the first byte, then the second, then the third, and so on. As soon
        ///         as
        ///         you find a byte that isn't the same for both strings, you know they are different and can return a negative
        ///         response immediately. If you make it through both strings without finding any bytes that differ, you know
        ///         the strings are the same and can return a positive result. This means that comparing two strings can take a
        ///         different amount of time depending on how much of the strings match.
        ///     </p>
        ///     <p>
        ///         For example, a standard comparison of the strings "xyzabc" and "abcxyz" would immediately see that the first
        ///         character is different and wouldn't  bother to check the rest of the string. On the other hand, when the
        ///         strings "aaaaaaaaaaB" and "aaaaaaaaaaZ" are compared, the comparison algorithm scans through the block of "a"
        ///         before it determines the strings are unequal.
        ///     </p>
        /// </remarks>
        public static bool SafeEquals(this byte[] a, byte[] b)
        {
            /* 
             The code uses the XOR "^" operator to compare integers for equality, instead of the "==" operator.
             The result of XORing two integers will be zero if and only if they are exactly the same.
             This is because 0 XOR 0 = 0, 1 XOR 1 = 0, 0 XOR 1 = 1, 1 XOR 0 = 1.
             If we apply that to all the bits in both integers, the result will be zero only if all the bits matched. 
             */
            var diff = a.Length ^ b.Length;
            for (var i = 0; (i < a.Length) && (i < b.Length); i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}