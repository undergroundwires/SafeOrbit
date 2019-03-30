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
        public void Register<T>(IStubProvider<T> stubProvider)
        {
            _typeFuncs.Add(typeof(T).FullName, () => stubProvider.Provide());
        }
        public void Register(Type type, IStubProvider stubProvider)
        {
            _typeFuncs.Add(type.FullName, stubProvider.Provide);
        }
        /// <summary>
        /// Registers all stub providers that implements <see cref="IStubProvider{T}"/> in the assembly.
        /// </summary>
        /// <param name="assembly">The assembly to look for instances that implements <see cref="IStubProvider{T}"/>.</param>
        public void RegisterAll(Assembly assembly)
        {
            var allStubProviderTypes = from type in assembly.GetTypes()
                                       where typeof(IStubProvider).GetTypeInfo().IsAssignableFrom(type)
                                       select type;
            foreach (var instanceProdiverType in allStubProviderTypes)
            {
                var instanceProvider = Activator.CreateInstance(instanceProdiverType) as IStubProvider;
                var stubType = instanceProdiverType
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
    }
}
