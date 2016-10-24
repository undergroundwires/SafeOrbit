using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SafeOrbit.Random.Library
{
    public class Xorshift
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void GetBytes(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            int offset = 0, offsetEnd = buffer.Length;
            uint x = 548787455, y = 842502087, z = 3579807591, w = 273326509;
            fixed (byte* pbytes = buffer)
            {
                uint* pbuf = (uint*)(pbytes + offset);
                uint* pend = (uint*)(pbytes + offsetEnd);
                while (pbuf < pend)
                {
                    uint tx = x ^ (x << 11);
                    uint ty = y ^ (y << 11);
                    uint tz = z ^ (z << 11);
                    uint tw = w ^ (w << 11);
                    *(pbuf++) = x = w ^ (w >> 19) ^ (tx ^ (tx >> 8));
                    *(pbuf++) = y = x ^ (x >> 19) ^ (ty ^ (ty >> 8));
                    *(pbuf++) = z = y ^ (y >> 19) ^ (tz ^ (tz >> 8));
                    *(pbuf++) = w = z ^ (z >> 19) ^ (tw ^ (tw >> 8));
                }
            }
        }
    }
}
