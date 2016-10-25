
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeOrbit.Common
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Appends the byte array after the existing one
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="byteArray"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="byteArrays"/> is <see langword="null" />.</exception>
        /// <exception cref="OverflowException">The sum of byte array length larger than <see cref="F:System.Int32.MaxValue" />.</exception>
        public static byte[] Combine(this byte[] byteArray, params byte[][] byteArrays)
        {
            if (byteArray == null) throw new ArgumentNullException(nameof(byteArray));
            if (byteArrays == null || byteArrays.Any(b => b == null)) throw new ArgumentNullException(nameof(byteArrays));
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
    }
}
