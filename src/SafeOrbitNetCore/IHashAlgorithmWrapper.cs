namespace SafeOrbit.Random.Tinhat
{
    public interface IHashAlgorithmWrapper
    {
        int HashSizeInBits { get; }

        byte[] ComputeHash(byte[] data);
        void Dispose();
    }
}