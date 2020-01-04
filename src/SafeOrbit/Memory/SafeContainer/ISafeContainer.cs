using System;
using SafeOrbit.Core.Protectable;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory
{
    /// <summary>
    ///     <p>Abstraction for a factory class that's protected in memory.</p>
    ///     <p>It abstracts strategies for different protection modes</p>
    /// </summary>
    /// <seealso cref="SafeContainerProtectionMode" />
    /// <seealso cref="SafeContainerProtectionMode" />
    public interface ISafeContainer :
        IAlerts,
        IProtectable<SafeContainerProtectionMode>,
        IServiceProvider
    {
        TComponent Get<TComponent>();

        void Register<TComponent>(LifeTime lifeTime = LifeTime.Transient) where TComponent : class, new();

        /// <summary>
        ///     Registers the specified life time.
        /// </summary>
        /// <typeparam name="TComponent">The type of the the component.</typeparam>
        /// <typeparam name="TImplementation">The type of the t implementation.</typeparam>
        /// <param name="lifeTime">The life time.</param>
        void Register<TComponent, TImplementation>(LifeTime lifeTime = LifeTime.Transient)
            where TComponent : class
            where TImplementation : TComponent, new();

        /// <summary>
        ///     Registers the specified instance initializer with an implementation.
        /// </summary>
        /// <typeparam name="TComponent">The type of the the component.</typeparam>
        /// <typeparam name="TImplementation">The type of the t implementation.</typeparam>
        /// <param name="instanceInitializer">The instance initializer.</param>
        /// <param name="lifeTime">The life time.</param>
        void Register<TComponent, TImplementation>(Func<TImplementation> instanceInitializer,
            LifeTime lifeTime = LifeTime.Unknown)
            where TComponent : class
            where TImplementation : TComponent, new();

        /// <summary>
        ///     Registers the specified instance initializer.
        /// </summary>
        /// <typeparam name="TComponent">The type of the the component.</typeparam>
        /// <param name="instanceInitializer">The instance initializer.</param>
        /// <param name="lifeTime">The life time.</param>
        void Register<TComponent>(Func<TComponent> instanceInitializer,
            LifeTime lifeTime = LifeTime.Unknown)
            where TComponent : class;

        /// <summary>
        ///     Verifies this instance.
        /// </summary>
        void Verify();
    }
}