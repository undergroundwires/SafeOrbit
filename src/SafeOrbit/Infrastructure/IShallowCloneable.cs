namespace SafeOrbit
{
    /// <summary>
    ///     Defines the interface for any object that can be shallow cloned.
    /// </summary>
    /// <typeparam name="TCloneable">The typed returned by the clone operation..</typeparam>
    public interface IShallowCloneable<out TCloneable> where TCloneable : class
    {
        /// <summary>
        ///     Creates a shallow clone of this object.
        /// </summary>
        /// <returns>A deep clone of this object.</returns>
        TCloneable ShallowClone();
    }
}