using System.Threading.Tasks;

namespace SafeOrbit
{
    /// <inheritdoc cref="IDeepCloneable{TCloneable}"/>
    /// <seealso cref="IDeepCloneable{TCloneable}"/>
    public interface IAsyncDeepCloneable<TCloneable> where TCloneable : class
    {
        /// <summary>
        ///     Creates a deep clone of this object.
        /// </summary>
        /// <returns>A deep clone of this object.</returns>
        Task<TCloneable> DeepCloneAsync();
    }
}