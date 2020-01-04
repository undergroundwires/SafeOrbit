using System;
using SafeOrbit.Exceptions.SerializableException;

#if !NETSTANDARD1_6
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif

namespace SafeOrbit.Exceptions
{
    /// <summary>
    /// An exception to throw when an <see cref="Enum"/> is out of range.
    /// </summary>
    /// <typeparam name="TEnum">Type of the enum</typeparam>
    /// <seealso cref="SafeOrbitException" />
    /// <seealso cref="SerializableExceptionBase" />
#if !NETSTANDARD1_6
    [Serializable]
#endif
    public class UnexpectedEnumValueException<TEnum> : SafeOrbitException where TEnum: Enum
    {
        public TEnum Value { get; set; }
        public UnexpectedEnumValueException(TEnum value)
            : base("Value " + value + " of enum " + typeof(TEnum).Name + " is not supported")
        {
            Value = value;
        }
#if !NETSTANDARD1_6
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        private UnexpectedEnumValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        protected override void ConfigureSerialize(ISerializationContext serializationContext)
        {
            serializationContext.Add(() => Value);
            base.ConfigureSerialize(serializationContext);
        }
#endif
    }
}