using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Providers
{
    /// <summary>
    ///     Provides the same instance of <typeparamref name="TImplementation" />.
    /// </summary>
    /// <see cref="LifeTime.Singleton" />
    /// <seealso cref="TransientInstanceProvider{TImplementation}" />
    /// <seealso cref="CustomInstanceProvider{TImplementation}" />
    /// <seealso cref="IInstanceProvider" />
    /// <seealso cref="SafeInstanceProviderBase{TImplementation}" />
    internal class SingletonInstanceProvider<TImplementation> : SafeInstanceProviderBase<TImplementation>
        where TImplementation : new()
    {
        private TImplementation _instance;

        public SingletonInstanceProvider(InstanceProtectionMode initialProtectionMode) : base(LifeTime.Singleton,
            initialProtectionMode)
        {
        }

        /// <summary>
        ///     Internal constructor with all dependencies.
        /// </summary>
        internal SingletonInstanceProvider(IInjectionDetector injectionDetector, InstanceProtectionMode protectionMode,
            InjectionAlertChannel alertChannel)
            : base(LifeTime.Singleton, injectionDetector, protectionMode, alertChannel)
        {
        }

        public override TImplementation GetInstance()
        {
            if (_instance == null) _instance = new TImplementation();
            return _instance;
        }
    }
}