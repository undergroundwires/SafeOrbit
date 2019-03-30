using System;
using SafeOrbit.Infrastructure.Reflection;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="ITypeIdGenerator" />
    public class TypeIdGeneratorFaker : StubProviderBase<ITypeIdGenerator>
    {
        public override ITypeIdGenerator Provide() => new FakeTypeIdGenerator();
        private class FakeTypeIdGenerator : ITypeIdGenerator
        {
            public string Generate<T>() => typeof(T).AssemblyQualifiedName;
            public string Generate(Type type) => type.AssemblyQualifiedName;
        }
    }

}