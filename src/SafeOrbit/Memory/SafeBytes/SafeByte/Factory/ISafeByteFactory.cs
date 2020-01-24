using System.Collections.Generic;
using System.Threading.Tasks;
using SafeOrbit.Memory.SafeBytesServices.Id;

namespace SafeOrbit.Memory.SafeBytesServices.Factory
{
    /// <summary>
    ///     Abstracts a factory that returns the right <see cref="ISafeByte" /> instance for any <see cref="byte" /> or
    ///     <see cref="ISafeByte.Id" />.
    /// </summary>
    internal interface ISafeByteFactory
    {
        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        ///     Returns the <see cref="ISafeByte" /> for the specified <see cref="byte" />.
        /// </summary>
        /// <param name="byte">The byte.</param>
        Task<ISafeByte> GetByByteAsync(byte @byte);

        /// <summary>
        ///     Returns the <see cref="ISafeByte" /> for the specified <see cref="ISafeByte.Id" />.
        /// </summary>
        /// <param name="safeByteId">The safe byte identifier.</param>
        /// <seealso cref="IByteIdGenerator" />
        Task<ISafeByte> GetByIdAsync(int safeByteId);

        /// <summary>
        ///    Creates <see cref="ISafeByte"/> variant of each byte in given <paramref name="stream"/>
        /// </summary>
        Task<IEnumerable<ISafeByte>> GetByBytesAsync(SafeMemoryStream stream);
    }
}