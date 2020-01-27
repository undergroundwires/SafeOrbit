using System;

namespace SafeOrbit.Exceptions.SerializableException
{
    /// <summary>
    ///     Abstracts serialization property.
    /// </summary>
    public interface ISerializationPropertyInfo
    {
        string PropertyName { get; }
        Type Type { get; }
        object Value { get; }
    }
}