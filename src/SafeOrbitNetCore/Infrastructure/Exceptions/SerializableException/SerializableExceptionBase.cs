/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;

#if NET46
using System.Security.Permissions;
using System.Linq;
using System.Runtime.Serialization;
#endif

namespace SafeOrbit.Exceptions.SerializableException
{
    /// <summary>
    ///     An abstract base for serializable classes.
    /// </summary>
    /// <remarks>
    ///     <p>Override <see cref="ConfigureSerialize" /> method to add different properties to the serialization.</p>
    /// </remarks>
    /// <seealso cref="Exception"/>
    /// <seealso cref="ConfigureSerialize"/>
#if NET46
/// <seealso cref="SerializableAttribute"/>
    [Serializable]
#endif
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

#if NET46
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

#if NET46

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

#if NET46
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