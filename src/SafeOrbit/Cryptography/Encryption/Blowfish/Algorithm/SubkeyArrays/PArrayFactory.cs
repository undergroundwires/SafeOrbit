namespace SafeOrbit.Cryptography.Encryption.Blowfish.Algorithm.SubkeyArrays
{
    internal class PBoxFactory
    {
        /// <summary>
        ///     The algorithm keeps subkey array P that's initialized with constant.
        /// </summary>
        /// <returns>P array of eighteen 32-bit integers</returns>
        /// <remarks>Increase the size of this array when increasing the number of rounds</remarks>
        public static uint[] GetPArray() => new uint[]
        {
            0x243f6a88, 0x85a308d3, 0x13198a2e, 0x03707344, 0xa4093822, 0x299f31d0,
            0x082efa98, 0xec4e6c89, 0x452821e6, 0x38d01377, 0xbe5466cf, 0x34e90c6c,
            0xc0ac29b7, 0xc97c50dd, 0x3f84d5b5, 0xb5470917, 0x9216d5d9, 0x8979fb1b
        };
    }
}