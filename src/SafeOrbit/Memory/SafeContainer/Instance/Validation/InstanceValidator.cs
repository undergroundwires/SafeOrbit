using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SafeOrbit.Extensions;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Validation
{
    internal class InstanceValidator : IInstanceValidator
    {
        public void ValidateAll(IReadOnlyList<IInstanceProvider> instanceProviders)
        {
            var rules = GetAllRules(instanceProviders).ToArray();
            foreach (var instanceProvider in instanceProviders) rules.ForEach(rule => Validate(rule, instanceProvider));
        }

        private void Validate(IInstanceProviderRule rule, IInstanceProvider instanceProvider)
        {
            if (rule.IsSatisfiedBy(instanceProvider)) return;
            var errors = string.Join(",", rule.Errors);
            if (rule.Type == RuleType.Warning) Debug.WriteLine($"{nameof(SafeContainer)} - Warning : {errors}");
            if (rule.Type == RuleType.Error) throw new ArgumentException(errors);
        }

        protected IEnumerable<IInstanceProviderRule> GetAllRules(IEnumerable<IInstanceProvider> instanceProviders)
        {
            yield return new SingletonShouldNotDependOnTransientRule(instanceProviders);
            yield return new SingletonShouldNotImplementIDisposable();
        }
    }
}