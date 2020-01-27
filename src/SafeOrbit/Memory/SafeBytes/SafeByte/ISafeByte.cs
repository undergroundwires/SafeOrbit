using System;
using System.Threading.Tasks;
using SafeOrbit.Common;

namespace SafeOrbit.Memory.SafeBytesServices
{
    internal interface ISafeByte : IDisposable,
        IEquatable<ISafeByte>, IAsyncEquatable<byte>,
        IDeepCloneable<ISafeByte>
    {
        int Id { get; }

        /// <summary>
        ///     Gets a value indicating whether any byte is set on this instance.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the byte is set; otherwise, <c>false</c>.
        /// </value>
        bool IsByteSet { get; }
        Task SetAsync(byte b);
        /// <summary>
        ///     Decrypts and returns the byte that this <see cref="SafeByte" /> instance represents.
        /// </summary>
        Task<byte> GetAsync();
    }
}