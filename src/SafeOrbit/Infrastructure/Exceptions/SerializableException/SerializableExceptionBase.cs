using System;

#if !NETSTANDARD1_6
using System.Security.Permissions;
using System.Linq;
using System.Runtime.Serialization;
#endif

namespace SafeOrbit.Exceptions.SerializableException
{
#pragma warning disable 1587
    /// <summary>
    ///     An abstract base for serializable classes.
    /// </summary>
    /// <remarks>
    ///     <p>Override <see cref="ConfigureSerialize" /> method to add different properties to the serialization.</p>
    /// </remarks>
    /// <seealso cref="Exception"/>
    /// <seealso cref="ConfigureSerialize"/>
#if !NETSTANDARD1_6
    /// <seealso cref="SerializableAttribute"/>
    [Serializable]
#endif
#pragma warning restore 587
    public abstract class SerializableExceptionBase : Exception
    {
        protected SerializableExceptionBase()
        {
        }

        protected SerializableExceptionBase(string message) : base(message)
        {
        }

        protected SerializableExceptionBase(string message, Exception inner) : base(message, inner)
        {
        }

        protected SerializableExceptionBase(Exception innerException)
            : base($"InnerException has occured. Check {nameof(InnerException)} property", innerException)
        {
        }

#if !NETSTANDARD1_6
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected SerializableExceptionBase(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ResourceReferenceProperty = info.GetString(nameof(ResourceReferenceProperty));
            var serializationContext = ConfigureAndGetSerializationContext();
            DeserializeProperties(serializationContext, info);
        }
#endif

        public string ResourceReferenceProperty { get; set; }

        protected virtual void ConfigureSerialize(ISerializationContext serializationContext)
        {
            serializationContext.Add(() => ResourceReferenceProperty);
        }

#if !NETSTANDARD1_6

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            var serializationContext = ConfigureAndGetSerializationContext();
            SerializeProperties(serializationContext, info);
            base.GetObjectData(info, context);
        }
#endif
        public override string ToString() => $"Message = {Message}";

        private ISerializationContext ConfigureAndGetSerializationContext()
        {
            var serializationContext = new SerializationContext();
            ConfigureSerialize(serializationContext);
            return serializationContext;
        }

#if !NETSTANDARD1_6
        private void SerializeProperties(ISerializationContext serializationContext, SerializationInfo info)
        {
            var propertiesToSerialize = serializationContext.PropertyInfos;
            if (propertiesToSerialize == null && !propertiesToSerialize.Any()) return;
            foreach (var propertyInfo in propertiesToSerialize)
            {
                info.AddValue(propertyInfo.PropertyName, propertyInfo.Value, propertyInfo.Type);
            }
        }
        private void DeserializeProperties(ISerializationContext serializationContext, SerializationInfo info)
        {
            var propertiesToDeserialize = serializationContext.PropertyInfos;
            if (propertiesToDeserialize == null && !propertiesToDeserialize.Any()) return;
            foreach (var propertyInfo in propertiesToDeserialize)
            {
                var propertyName = propertyInfo.PropertyName;
                var prop = this.GetType().GetProperty(propertyName);
                var propertyType = propertyInfo.Type;
                if (propertyType == null) continue; //nullable types are not necessary
                var value = info.GetValue(propertyName, propertyType);
                prop.SetValue(this, value);
            }
        }
#endif
    }
}