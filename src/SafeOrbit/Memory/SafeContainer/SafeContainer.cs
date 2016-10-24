using System;
using System.Collections.Generic;
using System.Linq;
using SafeOrbit.Common;
using SafeOrbit.Common.Reflection;
using SafeOrbit.Exceptions;
using SafeOrbit.Interfaces;
using SafeOrbit.Library;
using SafeOrbit.Memory.Common;
using SafeOrbit.Memory.Injection;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Memory.SafeContainerServices;
using SafeOrbit.Memory.SafeContainerServices.Instance;
using SafeOrbit.Memory.SafeContainerServices.Instance.Providers;
using SafeOrbit.Memory.SafeContainerServices.Instance.Validation;

namespace SafeOrbit.Memory
{
    /// <summary>
    ///     <see cref="SafeContainer" /> is a lightweight object container with memory protection.
    /// </summary>
    /// <seealso cref="ISafeContainer" />
    /// <seealso cref="IServiceProvider" />
    /// <seealso cref="IProtectionLevelSwitchProvider{TProtectionLevel}" />
    /// <seealso cref="SafeInstanceProviderBase{TImplementation}" />
    /// <seealso cref="InjectionDetector" />
    public class SafeContainer :
        ProtectionLevelSwitchProviderBase<SafeContainerProtectionMode>,
        ISafeContainer
    {
        private readonly IInstanceProviderFactory _instanceFactory;
        private readonly IInstanceValidator _instanceValidator;
        private readonly ISafeObjectFactory _safeObjectFactory;
        private ISafeObject<Dictionary<string, IInstanceProvider>> _typeInstancesSafe;
        private readonly ITypeKeyGenerator _typeKeyGenerator;
        private InjectionAlertChannel _alertChannel;
        private bool _isVerified;

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeContainer"/> class.
        /// </summary>
        public SafeContainer() : this(Defaults.ContainerProtectionMode)
        {
        }

        /// <summary>
        ///     Gets a new instance for <see cref="SafeContainer" />.
        /// </summary>
        /// <param name="protectionMode">The protection level of the factory.</param>
        public SafeContainer(SafeContainerProtectionMode protectionMode)
            : this(
                TypeKeyGenerator.StaticInstance,
                InstanceProviderFactory.StaticInstance,
                new InstanceValidator(),
                SafeObjectFactory.StaticInstance, protectionMode)
        {
        }

        internal SafeContainer(
            ITypeKeyGenerator typeKeyGenerator,
            IInstanceProviderFactory instanceFactory,
            IInstanceValidator instanceValidator,
            ISafeObjectFactory safeObjectFactory,
            SafeContainerProtectionMode protectionMode = Defaults.ContainerProtectionMode,
            InjectionAlertChannel alertChannel = Defaults.AlertChannel) : base(protectionMode)
        {
            if (typeKeyGenerator == null) throw new ArgumentNullException(nameof(typeKeyGenerator));
            if (instanceFactory == null) throw new ArgumentNullException(nameof(instanceFactory));
            if (instanceValidator == null) throw new ArgumentNullException(nameof(instanceValidator));
            if (safeObjectFactory == null) throw new ArgumentNullException(nameof(safeObjectFactory));
            _typeKeyGenerator = typeKeyGenerator;
            _instanceFactory = instanceFactory;
            _instanceValidator = instanceValidator;
            _safeObjectFactory = safeObjectFactory;
            _alertChannel = alertChannel;
            ChangeProtectionMode(new ProtectionLevelSwitchingContext<SafeContainerProtectionMode>(protectionMode));
            SetAlertChannelInternal(alertChannel);
        }

        /// <exception cref="ArgumentException"><see cref="Verify" /> is not called.</exception>
        /// <exception cref="MemoryInjectionException">If the object has been changed after last stamp.</exception>
        /// <exception cref="MemoryInjectionException">If the object has been changed after last stamp.</exception>
        /// <exception cref="KeyNotFoundException">If the <see cref="TComponent" /> is not registered.</exception>
        public TComponent Get<TComponent>()
        {
            return (TComponent) GetService(typeof(TComponent));
        }

        public void Register<TImplementation>(LifeTime lifeTime = LifeTime.Transient)
            where TImplementation : class, new()
        {
            Register<TImplementation, TImplementation>(lifeTime);
        }

        public void Register<TComponent, TImplementation>(LifeTime lifeTime = LifeTime.Transient)
            where TComponent : class
            where TImplementation : TComponent, new()
        {
            var protectionMode = GetInnerInstanceProtectionMode(CurrentProtectionMode);
            var instance = _instanceFactory.Get<TImplementation>(lifeTime, protectionMode, AlertChannel);
            RegisterInstanceInternal<TComponent>(instance);
        }

        /// <exception cref="ArgumentException">If no type is registered in the current <see cref="SafeContainer" /> instance.</exception>
        /// <exception cref="ArgumentException">If the instance of <see cref="SafeContainer" /> is already verified.</exception>
        public void Verify()
        {
            if (!_typeInstancesSafe.Object.Any())
                throw new ArgumentException(
                    $"No type is registered in the {nameof(SafeContainer)} before calling {nameof(Verify)}. Please register types using {nameof(Register)} method.");
            if (_isVerified)
                throw new ArgumentException($"This instance of {nameof(SafeContainer)} is already verified.");
#if DEBUG
            var instanceProviders = _typeInstancesSafe.Object.Values.ToList().AsReadOnly();
            _instanceValidator.ValidateAll(instanceProviders);
#endif
            _isVerified = true;
            _typeInstancesSafe.MakeReadOnly();
        }

        /// <summary>
        ///     Registers a component with an implantation from a custom <see cref="Func{TImplementation}" />.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="instanceInitializer">The instance getter.</param>
        /// <param name="lifeTime">The life time.</param>
        /// <exception cref="ArgumentNullException"><paramref name="instanceInitializer" /> is <see langword="null" />.</exception>
        public void Register<TComponent, TImplementation>(Func<TImplementation> instanceInitializer,
            LifeTime lifeTime = LifeTime.Unknown) where TComponent : class where TImplementation : TComponent, new()
        {
            if (instanceInitializer == null) throw new ArgumentNullException(nameof(instanceInitializer));
            var protectionMode = GetInnerInstanceProtectionMode(CurrentProtectionMode);
            var instance = _instanceFactory.Get(instanceInitializer, protectionMode, AlertChannel, lifeTime);
            RegisterInstanceInternal<TComponent>(instance);
        }

        /// <exception cref="ArgumentException"><see cref="Verify" /> is not called.</exception>
        /// <exception cref="MemoryInjectionException">If the object has been changed after last stamp.</exception>
        /// <exception cref="MemoryInjectionException">If the object has been changed after last stamp.</exception>
        /// <exception cref="KeyNotFoundException">If the <see cref="serviceType" /> is not registered.</exception>
        public object GetService(Type serviceType)
        {
            if (!_isVerified) throw new ArgumentException($"Please call {nameof(Verify)} before using the factory");
            base.SpinUntilSecurityModeIsAvailable();
            var key = _typeKeyGenerator.Generate(serviceType);
            IInstanceProvider instanceProvider;
            var success = _typeInstancesSafe.Object.TryGetValue(key, out instanceProvider);
            if (!success) throw new KeyNotFoundException($"{serviceType.FullName} is not registered.");
            var result = instanceProvider.Provide();
            return result;
        }

        /// <summary>
        ///     Registers a component with a custom <see cref="Func{TImplementation}" />.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component.</typeparam>
        /// <param name="instanceInitializer">The instance getter.</param>
        /// <param name="lifeTime">The life time.</param>
        /// <exception cref="ArgumentNullException"><paramref name="instanceInitializer" /> is <see langword="null" />.</exception>
        public void Register<TComponent>(Func<TComponent> instanceInitializer, LifeTime lifeTime = LifeTime.Unknown)
            where TComponent : class
        {
            var protectionMode = GetInnerInstanceProtectionMode(CurrentProtectionMode);
            if (instanceInitializer == null) throw new ArgumentNullException(nameof(instanceInitializer));
            var instance = _instanceFactory.Get(instanceInitializer, protectionMode, AlertChannel, lifeTime);
            RegisterInstanceInternal<TComponent>(instance);
        }

        private void RegisterInstanceInternal<TComponent>(IInstanceProvider instance) where TComponent : class
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            var key = _typeKeyGenerator.Generate<TComponent>();
            base.SpinUntilSecurityModeIsAvailable();
            if (_typeInstancesSafe.Object.ContainsKey(key))
                throw new ArgumentException($"{typeof(TComponent).FullName} is already registered.");
            _typeInstancesSafe.ApplyChanges(dic => { dic.Add(key, instance); });
        }


        private ISafeObject<Dictionary<string, IInstanceProvider>> CreateInnerDictionary(
            SafeContainerProtectionMode protectionMode, InjectionAlertChannel alertChannel,
            Dictionary<string, IInstanceProvider> initialObject = null)
        {
            var innerDictionaryProtection = GetInnerDictionaryObjectProtectionMode(protectionMode);
            var settings = new InitialSafeObjectSettings
            (
                protectionMode: innerDictionaryProtection,
                isReadOnly: false,
                initialValue: initialObject,
                alertChannel: alertChannel
            );
            var result = _safeObjectFactory.Get<Dictionary<string, IInstanceProvider>>(settings);
            return result;
        }

        #region [IProtectionLevelSwitchProvider]

        /// <summary>
        /// Changes the protection mode of the inner dictionary and its values.
        /// </summary>
        protected sealed override void ChangeProtectionMode(
            IProtectionLevelSwitchingContext<SafeContainerProtectionMode> context)
        {
            Dictionary<string, IInstanceProvider> newDictionary = null;
            if (_typeInstancesSafe != null)
            {
                var innerInstanceProtection = GetInnerInstanceProtectionMode(context.NewValue);
                newDictionary = _typeInstancesSafe.Object;
                newDictionary.Values.ForEach(instance => instance.SetProtectionMode(innerInstanceProtection));
                _typeInstancesSafe.Dispose();
            }
            //re-create inner safe object
            _typeInstancesSafe = CreateInnerDictionary(context.NewValue, this.AlertChannel, newDictionary);
        }

        private static InstanceProtectionMode GetInnerInstanceProtectionMode(SafeContainerProtectionMode protectionMode)
        {
            switch (protectionMode)
            {
                case SafeContainerProtectionMode.FullProtection:
                    return InstanceProtectionMode.StateAndCode;
                case SafeContainerProtectionMode.NonProtection:
                    return InstanceProtectionMode.NoProtection;
                default:
                    throw new ArgumentOutOfRangeException(nameof(protectionMode), protectionMode, null);
            }
        }

        private static SafeObjectProtectionMode GetInnerDictionaryObjectProtectionMode(
            SafeContainerProtectionMode protectionMode)
        {
            switch (protectionMode)
            {
                case SafeContainerProtectionMode.FullProtection:
                    return SafeObjectProtectionMode.JustState; //no need to protect type of the dictionary
                case SafeContainerProtectionMode.NonProtection:
                    return SafeObjectProtectionMode.NoProtection;
                //SafeObject will not be protected at all. It'll still provide thread-safety.
                default:
                    throw new ArgumentOutOfRangeException(nameof(protectionMode), protectionMode, null);
            }
        }

        #endregion

        /// <summary>
        ///     Gets or sets the alert channel for the inner <see cref="IInjectionDetector" />.
        /// </summary>
        /// <seealso cref="IAlerts" />
        /// <seealso cref="IInjectionDetector" />
        /// <seealso cref="CanAlert" />
        /// <value>The alert channel.</value>
        public virtual InjectionAlertChannel AlertChannel
        {
            get { return _alertChannel; }
            set
            {
                if (_alertChannel == value) return;
                SetAlertChannelInternal(value);
            }
        }

        /// <summary>
        ///     Gets if this instance is allowed to alert via <see cref="AlertChannel" />.
        /// </summary>
        /// <seealso cref="IAlerts" />
        /// <seealso cref="IInjectionDetector" />
        /// <seealso cref="AlertChannel" />
        /// <value>If this instance is allowed to alert.</value>
        public bool CanAlert => CurrentProtectionMode != SafeContainerProtectionMode.NonProtection;

        private void SetAlertChannelInternal(InjectionAlertChannel value)
        {
            _alertChannel = value;
            _typeInstancesSafe.AlertChannel = value;
            var dict = _typeInstancesSafe.Object;
            if (dict?.Values.Any(v => !v.AlertChannel.Equals(value)) == true)
            {
                _typeInstancesSafe.ApplyChanges(dic => dic?
                .Values
                .Where(c=> !c.AlertChannel.Equals(value))
                .ForEach(instance => instance.AlertChannel = value));
            }
        }
    }
}