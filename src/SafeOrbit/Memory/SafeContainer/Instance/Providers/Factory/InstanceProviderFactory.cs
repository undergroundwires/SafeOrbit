using System;
using System.Collections.Generic;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Providers
{
    /// <summary>
    /// Returns the right
    /// </summary>
    /// <seealso cref="IInstanceProviderFactory" />
    internal class InstanceProviderFactory : IInstanceProviderFactory
    {
        public static IInstanceProviderFactory StaticInstance => StaticInstanceLazy.Value;
        public static Lazy<InstanceProviderFactory> StaticInstanceLazy = new Lazy<InstanceProviderFactory>();
        public IInstanceProvider Get<TImplementation>(LifeTime lifeTime, InstanceProtectionMode protectionMode,
            InjectionAlertChannel alertChannel) where TImplementation : new()
        {
            IInstanceProvider instance;
            switch (lifeTime)
            {
                case LifeTime.Singleton:
                    instance = new SingletonInstanceProvider<TImplementation>(protectionMode);
                    break;
                case LifeTime.Transient:
                    instance = new TransientInstanceProvider<TImplementation>(protectionMode);
                    break;
                case LifeTime.Unknown:
                    throw new ArgumentOutOfRangeException($"You can only register {LifeTime.Unknown} instances with a {nameof(Func<IInstanceProvider>)} parameter.Please check overloaded method.s");
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifeTime), lifeTime, null);
            }
            instance.AlertChannel = alertChannel;
            return instance;
        }

        public IInstanceProvider Get<TImplementation>(Func<TImplementation> instanceGetter, InstanceProtectionMode protectionMode,
            InjectionAlertChannel alertChannel, LifeTime lifeTime = LifeTime.Unknown)
        {
            var result = new CustomInstanceProvider<TImplementation>(
                lifeTime: lifeTime,
                instanceGetter: instanceGetter,
                protectionMode: protectionMode)
            {
                AlertChannel = alertChannel
            };
            result.SetProtectionMode(protectionMode);
            return result;
        }
    }
}