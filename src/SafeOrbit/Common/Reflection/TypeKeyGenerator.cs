using System;
using System.Diagnostics;

namespace SafeOrbit.Common.Reflection
{
    /// <summary>
    /// Generates a key/id for each type
    /// </summary>
    /// <seealso cref="ITypeKeyGenerator" />
    public class TypeKeyGenerator : ITypeKeyGenerator
    {
        public static ITypeKeyGenerator StaticInstance => StaticInstanceLazy.Value;
        public static Lazy<TypeKeyGenerator> StaticInstanceLazy = new Lazy<TypeKeyGenerator>();
        public string Generate<T>() => typeof (T).AssemblyQualifiedName;
        [DebuggerHidden] public string Generate(Type type) => type.AssemblyQualifiedName;
    }
}