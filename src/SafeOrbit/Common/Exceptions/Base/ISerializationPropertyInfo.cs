using System;

namespace SafeOrbit.Exceptions
{
    public interface ISerializationPropertyInfo
    {
        string PropertyName { get; }
        Type Type { get; }
        object Value { get; }
    }
}