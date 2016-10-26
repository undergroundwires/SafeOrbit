namespace SafeOrbit.Memory.ProtectionMode
{
    /// <summary>
    ///     Abstracts a class that can work in different protection modes.
    /// </summary>
    /// <typeparam name="TProtectionLevel">The type of the protection model.</typeparam>
    public interface IProtectionSwitchable<TProtectionLevel>
        where TProtectionLevel : struct
    {
        /// <summary>
        ///     Gets the current protection mode.
        /// </summary>
        /// <value>The current protection mode.</value>
        TProtectionLevel CurrentProtectionMode { get; }

        /// <summary>
        ///     Sets the protection mode.
        /// </summary>
        /// <param name="objectProtectionMode">The object protection mode.</param>
        void SetProtectionMode(TProtectionLevel objectProtectionMode);
    }
}