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
        /// Initializes this instance.
        /// </summary>
        void Initialize();
        /// <summary>
        /// Returns the <see cref="ISafeByte"/> for the specified <see cref="byte"/>.
        /// </summary>
        /// <param name="byte">The byte.</param>
        ISafeByte GetByByte(byte @byte);
        /// <summary>
        /// Returns the <see cref="ISafeByte"/> for the specified <see cref="ISafeByte.Id"/>.
        /// </summary>
        /// <param name="safeByteId">The safe byte identifier.</param>
        /// <seealso cref="IByteIdGenerator"/>
        ISafeByte GetById(int safeByteId);
    }
}