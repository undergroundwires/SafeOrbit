using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SafeOrbit.Tests
{
    public class StubFactory : IStubFactory
    {
        private readonly Dictionary<string, Func<object>> _typeFuncs = new Dictionary<string, Func<object>>();
        public void Register<T>(IStubProvider<T> stubProvider) => Register(typeof(T).FullName, stubProvider);
        public void Register(Type type, IStubProvider stubProvider) => Register(type.FullName, stubProvider);
        /// <summary>
        /// Registers all stub providers that implements <see cref="IStubProvider{T}"/> in the assembly.
        /// </summary>
        /// <param name="assembly">The assembly to look for instances that implements <see cref="IStubProvider{T}"/>.</param>
        public void RegisterAll(Assembly assembly)
        {
            var allStubProviderTypes = from type in assembly.GetTypes()
                                       where typeof(IStubProvider).GetTypeInfo().IsAssignableFrom(type)
                                       select type;
            foreach (var instanceProviderType in allStubProviderTypes)
            {
                var instanceProvider = Activator.CreateInstance(instanceProviderType) as IStubProvider;
                var stubType = instanceProviderType
                    .GetInterfaces()
                    .Where(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IStubProvider<>))
                    .SelectMany(i => i.GetGenericArguments())
                    .FirstOrDefault();
                this.Register(stubType, instanceProvider);
            }
        }
        public TStub Provide<TStub>()
        {
            if(!_typeFuncs.ContainsKey(typeof(TStub).FullName))
                throw new KeyNotFoundException(
                    $"Stub \"{typeof(TStub).Name}\" is not registered.\nFull name {typeof(TStub).FullName}");
            return (TStub) _typeFuncs[typeof (TStub).FullName].Invoke();
        }

        private void Register(string typeName, IStubProvider stubProvider)
        {
            if (_typeFuncs.ContainsKey(typeName))
            {
                var existing = _typeFuncs[typeName];
                throw new ArgumentException(
                    $"Cannot register \"{stubProvider.GetType().FullName}\" for the type \"{typeName}" +
                    $"¨{Environment.NewLine}" +
                    $"The type is already registered with: {existing.GetType().FullName}");
            }
            _typeFuncs.Add(typeName, stubProvider.Provide);
        }
    }
}
