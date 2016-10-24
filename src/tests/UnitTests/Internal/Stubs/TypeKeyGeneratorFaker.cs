using System;
using Moq;
using SafeOrbit.Common.Reflection;
using SafeOrbit.Memory.SafeContainerServices;
using SafeOrbit.Tests;

namespace SafeOrbit.UnitTests
{
    public class TypeKeyGeneratorFaker : StubProviderBase<ITypeKeyGenerator>
    {
        public override ITypeKeyGenerator Provide() => new FakeTypeKeyGenerator();
    }
    public class FakeTypeKeyGenerator : ITypeKeyGenerator
    {
        public string Generate<T>() => typeof(T).AssemblyQualifiedName;
        public string Generate(Type type) => type.AssemblyQualifiedName;
    }
}