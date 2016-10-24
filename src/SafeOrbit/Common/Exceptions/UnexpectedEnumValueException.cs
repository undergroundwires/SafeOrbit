using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SafeOrbit.Exceptions
{
    /// <summary>
    /// An exception to throw when an <see cref="Enum"/> is out of range.
    /// </summary>
    /// <typeparam name="TEnum">Type of the enum</typeparam>
    /// <seealso cref="SafeOrbitException" />
    /// <seealso cref="SerializableExceptionBase" />
    [Serializable]
    public class UnexpectedEnumValueException<TEnum> : SafeOrbitException where TEnum: IComparable, IFormattable, IConvertible
    {
        public TEnum Value { get; set; }
        protected override void ConfigureSerialize(ISerializationContext serializationContext)
        {
            serializationContext.Add(() => Value);
            base.ConfigureSerialize(serializationContext);
        }
        public UnexpectedEnumValueException(TEnum value)
            : base("Value " + value + " of enum " + typeof(TEnum).Name + " is not supported")
        {
            Value = value;
        }
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        private UnexpectedEnumValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}