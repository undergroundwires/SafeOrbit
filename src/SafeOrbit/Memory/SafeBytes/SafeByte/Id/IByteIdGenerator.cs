namespace SafeOrbit.Memory.SafeBytesServices.Id
{
    /// <summary>
    /// An abstraction for a class that provides unique values for each byte.
    /// </summary>
    /// <seealso cref="IByteIdGenerator" />
    internal interface IByteIdGenerator
    {
        /// <summary>
        /// Generates a unique id for <see cref="byte"/> values.
        /// </summary>
        /// <param name="byte">The value that'll get a unique <see cref="int"/> value.</param>
        /// <returns>A <see cref="int"/> value that's unique for the <paramref name="@byte"/> parameter.</returns>
        int Generate(byte @byte);
    }
}