namespace SafeOrbit.Cryptography.Random
{
    /// <summary>
    ///     Abstracts fast and cryptographically secure random generator.
    /// </summary>
    /// <seealso cref="ICryptoRandom" />
    /// <seealso cref="FastRandom" />
    public interface IFastRandom : ICryptoRandom
    {
    }
}