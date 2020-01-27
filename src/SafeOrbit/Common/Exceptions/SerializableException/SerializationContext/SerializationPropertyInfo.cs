using System;

namespace SafeOrbit.Exceptions.SerializableException
{
    public class SerializationPropertyInfo : ISerializationPropertyInfo
    {
        public SerializationPropertyInfo(string propertyName, Type type, object value)
        {
            PropertyName = propertyName;
            Type = type;
            Value = value;
        }

        public string PropertyName { get; }
        public Type Type { get; }
        public object Value { get; }
    }
}