using System.Diagnostics;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Providers
{
    /// <summary>
    ///     Provides a new instance of <see cref="TImplementation" /> every time.
    /// </summary>
    /// <seealso cref="LifeTime.Transient"/>
    /// <seealso cref="SingletonInstanceProvider{TImplementation}"/>
    /// <seealso cref="CustomInstanceProvider{TImplementation}"/>
    /// <seealso cref="IInstanceProvider"/>
    /// <seealso cref="SafeInstanceProviderBase{TImplementation}"/>
    internal class TransientInstanceProvider<TImplementation> : SafeInstanceProviderBase<TImplementation> where TImplementation : new()
    {
        public TransientInstanceProvider(InstanceProtectionMode protectionMode) : base(LifeTime.Transient, protectionMode) { }
        [DebuggerHidden]
        public override TImplementation GetInstance()
        {
            return new TImplementation();
        }
        public override bool CanProtectState { get; } = false; //TODO: Add strategy for protection "initial state" for transient instances.
    }
}