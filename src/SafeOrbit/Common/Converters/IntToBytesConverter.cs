using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeOrbit.Converters
{
    /// <summary>
    /// Int to big endian bytes.
    /// </summary>
    public class IntToBytesConverter : IConverter<int, byte[]>
    {
        public byte[] Convert(int source)
        {
            var result = BitConverter.GetBytes(source);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(result);
            }
            return result;
        }
    }
}
