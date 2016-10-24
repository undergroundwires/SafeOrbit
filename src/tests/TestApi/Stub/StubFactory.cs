
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

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
                                       where typeof(IStubProvider).IsAssignableFrom(type)
                                       select type;
            foreach (var instanceProdiverType in allStubProviderTypes)
            {
                var instanceProvider = Activator.CreateInstance(instanceProdiverType) as IStubProvider;
                var stubType = instanceProdiverType
                    .GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IStubProvider<>))
                    .SelectMany(i => i.GetGenericArguments())
                    .FirstOrDefault();
                this.Register(stubType, instanceProvider);
            }
        }
        public TStub Provide<TStub>()
        {
            return (TStub) _typeFuncs[typeof (TStub).FullName].Invoke();
        }
    }
}
