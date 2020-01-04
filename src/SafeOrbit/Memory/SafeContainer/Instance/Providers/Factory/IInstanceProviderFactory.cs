using System;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Providers
{
    /// <summary>
    ///     An abstraction for a factory class to retrieve right <seealso cref="IInstanceProvider" /> instance for right
    ///     <see cref="LifeTime" /> and <see cref="SafeContainerProtectionMode" />.
    /// </summary>
    /// <seealso cref="IInstanceProvider" />
    /// <seealso cref="LifeTime" />
    /// <seealso cref="SafeContainerProtectionMode" />
    internal interface IInstanceProviderFactory
    {
        /// <summary>
        ///     Provides an <see cref="IInstanceProvider" /> for the specified <see cref="LifeTime" />
        /// </summary>
        /// <remarks>
        ///     <p>
        ///         Only accepted <see cref="LifeTime" />'s are <see cref="LifeTime.Transient" /> and
        ///         <see cref="LifeTime.Singleton" />.
        ///     </p>
        /// </remarks>
        /// <typeparam name="TImplementation">Type of the requested instance.</typeparam>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="protectionMode">Initial protection mode for the result instance.</param>
        /// <param name="alertChannel">Initial alert channel for the result instance.</param>
        /// <returns>A <see cref="IInstanceProvider" />.</returns>
        /// <seealso cref="IInstanceProvider" />
        /// <seealso cref="InstanceProtectionMode" />
        /// <seealso cref="InjectionAlertChannel" />
        /// <seealso cref="LifeTime" />
        IInstanceProvider Get<TImplementation>(LifeTime lifeTime, InstanceProtectionMode protectionMode,
            InjectionAlertChannel alertChannel) where TImplementation : new();

        /// <summary>
        ///     Provides an <see cref="IInstanceProvider" /> with a specified instance getter function.
        /// </summary>
        IInstanceProvider Get<TImplementation>(Func<TImplementation> instanceGetter,
            InstanceProtectionMode protectionMode, InjectionAlertChannel alertChannel,
            LifeTime lifeTime = LifeTime.Unknown);
    }
}