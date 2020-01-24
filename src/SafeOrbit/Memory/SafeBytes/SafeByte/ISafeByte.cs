using System;
using System.Threading.Tasks;
using SafeOrbit.Core;

namespace SafeOrbit.Memory.SafeBytesServices
{
    internal interface ISafeByte : IDisposable,
        IEquatable<ISafeByte>, IAsyncEquatable<byte>,
        IDeepCloneable<ISafeByte>
    {
        int Id { get; }
        bool IsByteSet { get; }
        Task SetAsync(byte b);
        /// <summary>
        ///     Decrypts and returns the byte that this <see cref="SafeByte" /> instance represents.
        /// </summary>
        Task<byte> GetAsync();
    }
}