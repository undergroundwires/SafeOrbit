using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Validation
{
    internal class SingletonShouldNotDependOnTransientRule : IInstanceProviderRule
    {
        private readonly IEnumerable<IInstanceProvider> _registeredInstanceProviders;

        /// <param name="registeredInstanceProviders">List of all of the registered instance providers.</param>
        public SingletonShouldNotDependOnTransientRule(IEnumerable<IInstanceProvider> registeredInstanceProviders)
        {
            _registeredInstanceProviders = registeredInstanceProviders ??
                                           throw new ArgumentNullException(nameof(registeredInstanceProviders));
        }

        public IEnumerable<string> Errors { get; private set; }
        public RuleType Type { get; } = RuleType.Error;

        public bool IsSatisfiedBy(IInstanceProvider instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (instance.LifeTime == LifeTime.Transient) return true;
            var type = instance.ImplementationType;
            var transientDependencies = GetAllRegisteredTransientDependencies(type, _registeredInstanceProviders);
            if (transientDependencies == null || !transientDependencies.Any()) return true;
            Errors = transientDependencies.Select(t =>
                $"{nameof(LifeTime.Singleton)} ({instance.ImplementationType.FullName}) is depended on {nameof(LifeTime.Transient)} ({instance.ImplementationType.FullName})");
            return false;
        }

        private IEnumerable<IInstanceProvider> GetAllRegisteredTransientDependencies(Type type,
            IEnumerable<IInstanceProvider> allInstanceProviders)
        {
            var parameterTypes = GetAllTypesFromConstructors(type);
            return
                from instanceProvider in allInstanceProviders
                where instanceProvider.LifeTime == LifeTime.Transient &&
                      parameterTypes.Contains(instanceProvider.ImplementationType)
                select instanceProvider;
        }

        private IEnumerable<Type> GetAllTypesFromConstructors(Type type)
        {
            var constructors =
                type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var parameters = constructors.SelectMany(c => c.GetParameters());
            var types = parameters.Select(p => p.ParameterType);
            return types;
        }
    }
}