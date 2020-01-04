namespace SafeOrbit.Cryptography.Encryption.Blowfish.Algorithm
{
    internal static class UintExtensions
    {
        public static byte GetFirstByte(this uint w) => (byte) (w / 256 / 256 / 256 % 256);
        public static byte GetSecondByte(this uint w) => (byte) (w / 256 / 256 % 256);
        public static byte GetThirdByte(this uint w) => (byte) (w / 256 % 256);
        public static byte GetFourthByte(this uint w) => (byte) (w % 256);
    }
}