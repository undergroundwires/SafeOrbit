using System;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Providers
{
    /// <summary>
    ///     Provides an instance from a custom <see cref="Func{TResult}" />
    /// </summary>
    /// <seealso cref="SingletonInstanceProvider{TImplementation}"/>
    /// <seealso cref="TransientInstanceProvider{TImplementation}"/>
    /// <seealso cref="IInstanceProvider"/>
    /// <seealso cref="SafeInstanceProviderBase{TImplementation}"/>
    internal class CustomInstanceProvider<TImplementation> : SafeInstanceProviderBase<TImplementation>
    {
        private readonly Func<TImplementation> _instanceGetter;

        public CustomInstanceProvider(Func<TImplementation> instanceGetter, InstanceProtectionMode protectionMode, LifeTime lifeTime = LifeTime.Unknown)
            : base(lifeTime, protectionMode)
        {
            if (instanceGetter == null) throw new ArgumentNullException(nameof(instanceGetter));
            _instanceGetter = instanceGetter;
        }
        /// <summary>
        /// Returns a service object given the specified instance.
        /// </summary>
        /// <returns>TInstanceType.</returns>
        public override TImplementation GetInstance()
        {
            return _instanceGetter.Invoke();
        }

        public override bool CanProtectState => this.LifeTime == LifeTime.Singleton;
    }
}