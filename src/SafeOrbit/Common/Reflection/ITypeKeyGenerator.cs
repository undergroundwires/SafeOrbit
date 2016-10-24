using System;

namespace SafeOrbit.Common.Reflection
{
    public interface ITypeKeyGenerator
    {
        string Generate<T>();
        string Generate(Type type);
    }
}