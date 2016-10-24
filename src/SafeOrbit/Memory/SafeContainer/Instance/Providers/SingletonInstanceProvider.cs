using System.Diagnostics;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Providers
{
    /// <summary>
    ///     Provides the same instance of <see cref="TImplementation" />.
    /// </summary>
    /// <see cref="LifeTime.Singleton"/>
    /// <seealso cref="TransientInstanceProvider{TImplementation}"/>
    /// <seealso cref="CustomInstanceProvider{TImplementation}"/>
    /// <seealso cref="IInstanceProvider"/>
    /// <seealso cref="SafeInstanceProviderBase{TImplementation}"/>
    internal class SingletonInstanceProvider<TImplementation> : SafeInstanceProviderBase<TImplementation> where TImplementation : new()
    {
        private TImplementation _instance;
        public SingletonInstanceProvider(InstanceProtectionMode initialProtectionMode) : base(LifeTime.Singleton, initialProtectionMode) { }
        [DebuggerHidden]
        public override TImplementation GetInstance()
        {
            if(_instance == null) _instance = new TImplementation();
            return _instance;
        }
    }
}