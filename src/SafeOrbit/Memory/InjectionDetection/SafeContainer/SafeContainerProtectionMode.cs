namespace SafeOrbit.Memory
{
    /// <summary>
    ///     Protection levels for <see cref="ISafeContainer" /> instances.
    /// </summary>
    /// <seealso cref="ISafeContainer" />
    public enum SafeContainerProtectionMode
    {
        /// <summary>
        ///     Protection for both state and code of its instances.
        ///     It's slowest and the safest mode.
        ///     <seealso cref="InjectionType.CodeAndVariableInjection" />
        ///     <seealso cref="IInjectionDetector" />
        /// </summary>
        FullProtection,

        /// <summary>
        ///     No protection for either the code or the state of the objects.
        ///     It's much faster than <see cref="FullProtection" /> but provides no security against injections.
        /// </summary>
        NonProtection
    }
}