namespace SafeOrbit.Random.Tinhat
{
    public interface IHashAlgorithmWrapper1
    {
        int HashSizeInBits { get; }

        byte[] ComputeHash(byte[] data);
        void Dispose();
    }
}