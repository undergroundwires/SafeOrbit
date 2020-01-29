using System;
using System.Threading.Tasks;
using SafeOrbit.Common;

namespace SafeOrbit.Memory
{
    public interface ISafeBytes : IDisposable, IAsyncDeepCloneable<ISafeBytes>,
        IAsyncEquatable<byte[]>, IAsyncEquatable<ISafeBytes>
    {
        /// <summary>
        ///     Returns to real length of the bytes inside
        /// </summary>
        int Length { get; }
        bool IsDisposed { get; }
        /// <summary>
        ///     Adds the byte <paramref name="@byte"/> to the end of the list.
        /// </summary>
        Task AppendAsync(byte @byte);
        /// <summary>
        ///     Adds the given encrypted bytes t the end of the list.
        /// </summary>
        Task AppendAsync(ISafeBytes safeBytes);
        /// <summary>
        ///     Adds the given bytes at the end of the list.
        /// </summary>
        Task AppendManyAsync(SafeMemoryStream stream);
        /// <summary>
        ///     Gets single byte in the safe list
        /// </summary>
        /// <param name="position">Index of the byte</param>
        /// <returns>Byte from the array</returns>
        Task<byte> RevealDecryptedByteAsync(int position);
        int GetHashCode();
        Task<byte[]> RevealDecryptedBytesAsync();
    }
}