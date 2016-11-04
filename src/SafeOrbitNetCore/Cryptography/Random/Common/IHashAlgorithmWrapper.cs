namespace SafeOrbit.Cryptography.Random.Common
{
    internal interface IHashAlgorithmWrapper
    {
        int HashSizeInBits { get; }

        byte[] ComputeHash(byte[] data);
        void Dispose();
    }
}