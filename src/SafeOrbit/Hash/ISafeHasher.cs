namespace SafeOrbit.Hash
{
    /// <summary>
    /// An interface for a fast and cryptological hasher
    /// </summary>
    public interface ISafeHasher<out TResult, in TSeed>
    {
        TResult Compute(byte[] input);
    }

    public interface ISafeHasher : ISafeHasher<byte[], uint>
    {

    }
}