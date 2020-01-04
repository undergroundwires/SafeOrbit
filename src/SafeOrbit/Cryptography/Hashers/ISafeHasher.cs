namespace SafeOrbit.Cryptography.Hashers
{
    /// <summary>
    ///     Abstracts interface for a slow cryptographic hash function.
    /// </summary>
    public interface ISafeHasher<out TResult>
    {
        TResult Compute(byte[] input);
    }

    /// <summary>
    ///     Abstracts interface for a slow cryptographic hash function returning an array of bytes.
    /// </summary>
    public interface ISafeHasher : ISafeHasher<byte[]>
    {
    }
}