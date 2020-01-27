using System;

namespace SafeOrbit.Memory.InjectionServices.Protectable
{
    /// <summary>
    ///     Defines a class that can work in different protection modes, and can dynamically switch between them.
    /// </summary>
    /// <typeparam name="TProtectionLevel">The type of the protection model.</typeparam>
    public interface IProtectable<TProtectionLevel>
        where TProtectionLevel : Enum
    {
        /// <summary>
        ///     Gets the current protection mode.
        /// </summary>
        /// <value>The current protection mode.</value>
        TProtectionLevel CurrentProtectionMode { get; }

        /// <summary>
        ///     Sets the protection mode.
        /// </summary>
        /// <param name="mode">The object protection mode.</param>
        void SetProtectionMode(TProtectionLevel mode);
    }
}