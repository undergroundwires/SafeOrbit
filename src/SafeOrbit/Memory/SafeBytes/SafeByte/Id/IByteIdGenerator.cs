using System.Collections.Generic;

namespace SafeOrbit.Memory.SafeBytesServices.Id
{
    /// <summary>
    ///     An abstraction for a class that provides unique values for each byte.
    /// </summary>
    /// <seealso cref="IByteIdGenerator" />
    internal interface IByteIdGenerator
    {
        /// <summary>
        ///     Generates a unique id for <see cref="byte" /> values.
        /// </summary>
        /// <param name="byte">The value that'll get a unique <see cref="int" /> value.</param>
        /// <returns>A <see cref="int" /> value that's unique for the <paramref name="byte" /> parameter.</returns>
        int Generate(byte @byte);

        /// <summary>
        ///     Generates list of unique id for each <see cref="byte" /> in <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Stream from within all byte ids will be generated</param>
        /// <returns>List of <see cref="int" /> value that are unique for the bytes in <paramref name="stream" />.</returns>
        IEnumerable<int> GenerateMany(SafeMemoryStream stream);
    }
}