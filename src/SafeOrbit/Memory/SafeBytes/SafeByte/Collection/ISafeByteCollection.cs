using System;
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
        int Length { get; }
        void Append(ISafeByte safeByte);

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