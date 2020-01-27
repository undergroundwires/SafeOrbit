using System;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Memory.InjectionServices.Protectable;

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
        /// <summary>
        ///     Gets the requested component/service.
        /// </summary>
        /// <typeparam name="TComponent">The type of the the component.</typeparam>
        /// <returns>Requested component.</returns>
        TComponent Get<TComponent>();

        /// <summary>
        ///     Registers the specified life time.
        /// </summary>
        /// <typeparam name="TComponent">The type of the t implementation.</typeparam>
        /// <param name="lifeTime">The life time.</param>
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
        ///     Registers a component with an implantation from a custom <see cref="Func{TImplementation}" />.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="instanceInitializer">The instance getter.</param>
        /// <param name="lifeTime">The life time.</param>
        void Register<TComponent, TImplementation>(Func<TImplementation> instanceInitializer,
            LifeTime lifeTime = LifeTime.Unknown)
            where TComponent : class
            where TImplementation : TComponent, new();

        /// <summary>
        ///     Registers a component with a custom <see cref="Func{TComponent}" />.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component.</typeparam>
        /// <param name="instanceInitializer">The instance getter.</param>
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