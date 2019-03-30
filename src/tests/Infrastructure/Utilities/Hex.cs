using System;
using System.Linq;

namespace SafeOrbit.Tests
{
    public static class Hex
    {
        public static byte[] DecodeHexToBytes(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}