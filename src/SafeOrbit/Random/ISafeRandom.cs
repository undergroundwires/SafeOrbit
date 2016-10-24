using SafeOrbit.Interfaces;

namespace SafeOrbit.Random
{
    public interface ISafeRandom : IRandom
    {
        void Seed(byte[] seed);
        byte[] GetNonZeroBytes(int length);
    }
}