using System;
using SafeOrbit.Memory;

namespace SafeOrbit.Memory.SafeBytesServices.Collection
{
    /// <summary>
    ///     Abstraction of a collection for <see cref="ISafeByte"/> instances.
    /// </summary>
    /// <seealso cref="ISafeByte"/>
    internal interface ISafeByteCollection : IDisposable
    {
        int Length { get; }
        void Append(ISafeByte safeByte);
        ISafeByte Get(int index);
        /// <summary>
        /// Returns all of the real byte values that <see cref="ISafeByteCollection"/> holds.
        /// Reveals all protected data in memory.
        /// Usage with <see cref="SafeMemoryStream"/> is recommended.
        /// </summary>
        /// <returns></returns>
        byte[] ToDecryptedBytes();
    }
}