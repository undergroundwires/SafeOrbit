namespace SafeOrbit.Cryptography.Random.RandomGenerators.Crypto.Utilities
{
    internal class Pack
    {
        internal static ulong BE_To_UInt64(byte[] bs, int off)
        {
            uint hi = BE_To_UInt32(bs, off);
            uint lo = BE_To_UInt32(bs, off + 4);
            return ((ulong)hi << 32) | (ulong)lo;
        }
        internal static uint BE_To_UInt32(byte[] bs, int off)
        {
            uint n = (uint)bs[off] << 24;
            n |= (uint)bs[++off] << 16;
            n |= (uint)bs[++off] << 8;
            n |= (uint)bs[++off];
            return n;
        }
        internal static void UInt64_To_BE(ulong n, byte[] bs, int off)
        {
            UInt32_To_BE((uint)(n >> 32), bs, off);
            UInt32_To_BE((uint)(n), bs, off + 4);
        }
        internal static void UInt32_To_BE(uint n, byte[] bs, int off)
        {
            bs[off] = (byte)(n >> 24);
            bs[++off] = (byte)(n >> 16);
            bs[++off] = (byte)(n >> 8);
            bs[++off] = (byte)(n);
        }
    }
}