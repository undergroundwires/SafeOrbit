
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Validation
{
    internal class SingletonShouldNotDependOnTransientRule : IInstanceProviderRule
    {
        public IEnumerable<string> Errors { get; private set; }
        public RuleType Type { get; } = RuleType.Error;
        private readonly IEnumerable<IInstanceProvider> _registeredInstanceProviders;
        /// <param name="registeredInstanceProviders">List of all of the registered instance providers.</param>
        public SingletonShouldNotDependOnTransientRule(IEnumerable<IInstanceProvider> registeredInstanceProviders)
        {
            if (registeredInstanceProviders == null) throw new ArgumentNullException(nameof(registeredInstanceProviders));
            _registeredInstanceProviders = registeredInstanceProviders;
        }
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
        private IEnumerable<IInstanceProvider> GetAllRegisteredTransientDependencies(Type type, IEnumerable<IInstanceProvider> allInstanceProviders)
        {
            var parameterTypes = GetAllTypesFromConstructors(type);
            return
                from instanceProvider in allInstanceProviders
                where instanceProvider.LifeTime == LifeTime.Transient && parameterTypes.Contains(instanceProvider.ImplementationType)
                select instanceProvider;
        }
        private IEnumerable<Type> GetAllTypesFromConstructors(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var parameters = constructors.SelectMany(c => c.GetParameters());
            var types = parameters.Select(p => p.ParameterType);
            return types;
        } 
    }
}
