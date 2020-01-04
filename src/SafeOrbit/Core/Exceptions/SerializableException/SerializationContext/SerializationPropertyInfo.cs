using System;

namespace SafeOrbit.Exceptions.SerializableException
{
    public class SerializationPropertyInfo : ISerializationPropertyInfo
    {
        public string PropertyName { get; private set; }
        public Type Type { get; private set; }
        public object Value { get; private set; }
        public SerializationPropertyInfo(string propertyName, Type type, object value)
        {
            PropertyName = propertyName;
            Type = type;
            Value = value;
        }
    }
}