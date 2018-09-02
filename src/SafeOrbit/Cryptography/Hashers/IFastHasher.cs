namespace SafeOrbit.Cryptography.Hashers
{
    /// <summary>
    ///     Abstracts a fast and cryptographic hash function
    /// </summary>
    public interface IFastHasher<out TResult, in TSeed>
    {
        TResult ComputeFast(byte[] input);
        TResult ComputeFast(byte[] input, TSeed seed);
    }

    /// <summary>
    ///     Abstracts a fast cryptographic hash function that returns <see cref="int" /> and can be seeded by
    ///     <see cref="uint" />.
    /// </summary>
    public interface IFastHasher : IFastHasher<int, uint>
    {
    }
}