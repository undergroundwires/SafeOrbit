using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeOrbit.Memory.SafeBytesServices.Collection
{
    /// <inheritdoc />
    /// <summary>
    ///     Abstraction of a collection for <see cref="T:SafeOrbit.Memory.SafeBytesServices.ISafeByte" /> instances.
    /// </summary>
    /// <seealso cref="T:SafeOrbit.Memory.SafeBytesServices.ISafeByte" />
    internal interface ISafeByteCollection : IDisposable
    {
        /// <summary>
        ///     Gets the length.
        /// </summary>
        /// <value>The length.</value>
        int Length { get; }
        /// <summary>
        ///     Appends the specified <see cref="ISafeByte" /> instance to the inner encrypted collection.
        /// </summary>
        /// <param name="safeByte">The safe byte.</param>
        /// <seealso cref="ISafeByte" />
        void Append(ISafeByte safeByte);
        /// <summary>
        ///     Appends the list of <see cref="ISafeByte" /> to the end of the collection
        /// </summary>
        /// <param name="safeBytes">Bytes to append.</param>
        /// <seealso cref="ISafeByte" />
        void AppendMany(IEnumerable<ISafeByte> safeBytes);
        /// <summary>
        ///     Gets the byte as <see cref="ISafeByte" /> for the specified index asynchronously.
        /// </summary>
        /// <param name="index">The position of the byte.</param>
        Task<ISafeByte> GetAsync(int index);

        /// <summary>
        ///     Returns all of the real byte values that <see cref="ISafeByteCollection" /> holds.
        ///     CAUTION: Reveals all protected data in memory. Use with <see cref="SafeMemoryStream" />.
        /// </summary>
        byte[] ToDecryptedBytes();
    }
}