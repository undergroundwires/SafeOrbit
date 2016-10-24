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
