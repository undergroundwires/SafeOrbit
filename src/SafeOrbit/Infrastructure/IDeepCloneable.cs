namespace SafeOrbit
{
    /// <summary>
    ///     Defines the interface for any object that can be deep cloned.
    /// </summary>
    /// <typeparam name="TCloneable">The typed returned by the clone operation.</typeparam>
    public interface IDeepCloneable<out TCloneable> where TCloneable : class
    {
        /// <summary>
        ///     Creates a deep clone of this object.
        /// </summary>
        /// <returns>A deep clone of this object.</returns>
        TCloneable DeepClone();
    }
}