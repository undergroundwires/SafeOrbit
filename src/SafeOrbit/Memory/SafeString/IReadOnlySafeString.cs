using System.Threading.Tasks;
using SafeOrbit.Common;
using SafeOrbit.Memory.SafeStringServices;

namespace SafeOrbit.Memory
{
    /// <summary>
    ///     Immutable encrypted string.
    /// </summary>
    public interface IReadOnlySafeString: IAsyncEquatable<string>, IAsyncEquatable<IReadOnlySafeString>
    {
        /// <summary>
        ///     Gets whether the current <see cref="IReadOnlySafeString"/> is disposed.
        /// </summary>
        bool IsDisposed { get; }
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
        ///     Reveals the string as a disposable string.
        /// </summary>
        /// <example>
        ///     <code>
        ///           using(var secret = await safeString.RevealDecryptedStringAsync())
        ///           {
        ///             // Use secret.String here.  While in the 'using' block, the string is accessible
        ///             // but pinned in memory.  When the 'using' block terminates, the string is zeroed
        ///             // out for security, and garbage collected as usual.
        ///           }
        ///     </code>
        /// </example>
        Task<IDisposableString> RevealDecryptedStringAsync();

        /// <summary>
        ///     Gets encrypted bytes of a character at given <paramref name="index"/>.
        /// </summary>
        IReadOnlySafeBytes GetAsSafeBytes(int index);

        /// <inheritdoc cref="object.GetHashCode"/>
        int GetHashCode();
    }
}
