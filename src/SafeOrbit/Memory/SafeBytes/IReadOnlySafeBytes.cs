using System.Threading.Tasks;
using SafeOrbit.Common;

namespace SafeOrbit.Memory
{
    /// <summary>
    ///     Immutable encrypted byte array.
    /// </summary>
    public interface IReadOnlySafeBytes:
        IAsyncDeepCloneable<ISafeBytes>, IAsyncEquatable<byte[]>, IAsyncEquatable<IReadOnlySafeBytes>
    {
        /// <summary>
        ///     Returns to real length of the bytes inside
        /// </summary>
        int Length { get; }
        
        /// <summary>
        ///     Gets decrypted single byte at given <paramref name="position"/> in the safe list
        /// </summary>
        /// <param name="position">Index of the byte</param>
        /// <returns>Byte from the array</returns>
        Task<byte> RevealDecryptedByteAsync(int position);

        /// <summary>
        ///     Gets all plain bytes that this instance encrypts.
        /// </summary>
        Task<byte[]> RevealDecryptedBytesAsync();

        /// <inheritdoc cref="object.GetHashCode"/>
        int GetHashCode();
        /// <summary>
        ///     Gets whether the current <see cref="ISafeBytes"/> is disposed.
        /// </summary>
        bool IsDisposed { get; }
    }
}
