namespace SafeOrbit.Cryptography.Random.SafeRandomServices
{
    internal interface IHashAlgorithmWrapper1
    {
        int HashSizeInBits { get; }

        byte[] ComputeHash(byte[] data);
        void Dispose();
    }
}