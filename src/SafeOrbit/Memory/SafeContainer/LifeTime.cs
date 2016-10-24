namespace SafeOrbit.Memory
{
    /// <summary>
    /// Specifies the lifetime of a service in an <see cref="IInstanceProvider"/>.
    /// </summary>
    /// <seealso cref="IInstanceProvider"/>
    public enum LifeTime
    {
        /// <summary>
        /// <see cref="Singleton"/> life time is a static, single instance of the class.
        /// <see cref="SafeContainer"/> returns the same instance for each request.
        /// Instances that are declared as <see cref="Singleton"/> should be thread-safe in a multi-threaded environment.
        /// </summary>
        Singleton,
        /// <summary>
        /// Transient are created each time they are requested.
        /// <see cref="SafeContainer"/> returns a new instance of the for after each request.
        /// </summary>
        Transient,
        /// <summary>
        /// Represents lifetime that's unknown to the SafeContainer.
        /// </summary>
        Unknown
    }
}