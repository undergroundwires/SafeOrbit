using System;
using System.Threading.Tasks;
using SafeOrbit.Library.Build;
using SafeOrbit.Memory;
using SafeOrbit.Memory.Injection;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Memory.SafeBytesServices.Factory;

namespace SafeOrbit.Library
{
    /// <summary>
    ///     The class where you control the current library behavior.
    /// </summary>
    /// <seealso cref="Current" />
    /// <seealso cref="ISafeOrbitCore" />
    /// <seealso cref="IAlerts" />
    public class SafeOrbitCore : ISafeOrbitCore, IAlerts
    {
        public const SafeContainerProtectionMode DefaultInnerFactoryProtectionMode =
            SafeContainerProtectionMode.FullProtection;

        private static readonly Lazy<SafeOrbitCore> CurrentLazy = new Lazy<SafeOrbitCore>(() => new SafeOrbitCore());

        /// <summary>
        ///     Use static <see cref="Current" /> property instead.
        /// </summary>
        internal SafeOrbitCore()
        {
            Factory = SetupFactory();
        }

        /// <summary>
        ///     Returns the current <see cref="SafeOrbitCore" /> class for running SafeOrbit.
        /// </summary>
        public static SafeOrbitCore Current => CurrentLazy.Value;

        /// <inheritdoc />
        public InjectionAlertChannel AlertChannel
        {
            get => Factory.AlertChannel;
            set => Factory.AlertChannel = value;
        }

        /// <inheritdoc />
        public bool CanAlert => Factory.CanAlert;

        /// <inheritdoc />
        public event EventHandler<IInjectionMessage> LibraryInjected;

        /// <inheritdoc />
        public ISafeContainer Factory { get; }

        /// <inheritdoc />
        public IBuildInfo BuildInfo => new BuildInfo();

        /// <inheritdoc />
        public async Task StartEarlyAsync()
        {
            var factory = Factory.Get<ISafeByteFactory>();
            await factory.InitializeAsync() // Here we also initialize the session salt, random generators, entropy collectors.
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Internal method to alert an injection. It's virtual for testability.
        /// </summary>
        /// <param name="object">The injected object.</param>
        /// <param name="info">The information.</param>
        internal virtual void AlertInjection(object @object, IInjectionMessage info)
        {
            var localCopy = LibraryInjected;
            localCopy?.Invoke(@object, info);
        }

        private static ISafeContainer SetupFactory()
        {
            var result = new SafeContainer(DefaultInnerFactoryProtectionMode);
            FactoryBootstrapper.Bootstrap(result);
            result.Verify();
            return result;
        }
    }
}