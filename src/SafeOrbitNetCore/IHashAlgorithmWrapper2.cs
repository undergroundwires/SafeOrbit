using System;

namespace SafeOrbit.Cryptography.Random.SafeRandomServices
{
    public interface IHashAlgorithmWrapper: IDisposable
    {
        int HashSizeInBits { get; }
        byte[] ComputeHash(byte[] data);
        void Dispose();
    }
}