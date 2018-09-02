using System;
using System.Collections.Generic;
using System.Reflection;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Validation
{
    internal class SingletonShouldNotImplementIDisposable : IInstanceProviderRule
    {
        public IEnumerable<string> Errors { get; private set; }
        public RuleType Type { get; } = RuleType.Warning;
        public bool IsSatisfiedBy(IInstanceProvider instance)
        {
            if (instance.LifeTime == LifeTime.Transient) return true;
            var type = instance.ImplementationType;
            if (type == null) throw new ArgumentException($"Type ({type.FullName}) could not be loaded");
            var isDisposable = typeof(IDisposable).GetTypeInfo().IsAssignableFrom(type);
            if (!isDisposable) return true;
            Errors = new[] { $"{type.AssemblyQualifiedName} implements {nameof(IDisposable)} but registered as {nameof(LifeTime.Singleton)}" };
            return false;
        }
    }
}