namespace SafeOrbit.Cryptography.Random.RandomGenerators
{
    internal interface IHashAlgorithmWrapper
    {
        int HashSizeInBits { get; }

        byte[] ComputeHash(byte[] data);
        void Dispose();
    }
}