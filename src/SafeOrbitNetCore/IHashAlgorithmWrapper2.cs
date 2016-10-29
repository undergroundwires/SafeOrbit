namespace SafeOrbit.Cryptography.Random.SafeRandomServices
{
    internal interface IHashAlgorithmWrapper
    {
        int HashSizeInBits { get; }

        byte[] ComputeHash(byte[] data);
        void Dispose();
    }
}