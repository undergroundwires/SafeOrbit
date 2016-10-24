using SafeOrbit.Interfaces;

namespace SafeOrbit.Random
{
    public interface IFastRandom : IRandom
    {
        void Reseed(int seed);
    }
}