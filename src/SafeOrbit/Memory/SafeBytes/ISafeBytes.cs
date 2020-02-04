using System;
using System.Threading.Tasks;

namespace SafeOrbit.Memory
{
    /// <summary>
    ///     Adds mutability to <seealso cref="IReadOnlySafeBytes"/>.
    /// </summary>
    public interface ISafeBytes : IReadOnlySafeBytes, IDisposable
    {
        /// <summary>
        ///     Adds the byte <paramref name="byte"/> to the end of the list.
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
    }
}