using System.Threading.Tasks;
using SafeOrbit.Common;

namespace SafeOrbit.Memory
{
    /// <summary>
    ///     Immutable encrypted string.
    /// </summary>
    public interface IReadOnlySafeString: IAsyncEquatable<string>, IAsyncEquatable<IReadOnlySafeString>
    {
        /// <summary>
        ///     Gets whether the current instance holds any characters.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        ///     Gets the number of characters in the current instance.
        /// </summary>
        int Length { get; }

        /// <summary>
        ///     Reveals the decrypted <see cref="char"/> at given <paramref name="index"/>.
        /// </summary>
        Task<char> RevealDecryptedCharAsync(int index);

        /// <summary>
        ///     Reveals the string as decrypted plain bytes.
        /// </summary>
        Task<byte[]> RevealDecryptedBytesAsync();

        /// <summary>
        ///     Gets encrypted bytes of a character at given <paramref name="index"/>.
        /// </summary>
        IReadOnlySafeBytes GetAsSafeBytes(int index);

        /// <inheritdoc cref="object.GetHashCode"/>
        int GetHashCode();
    }
}
