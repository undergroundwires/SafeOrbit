using System;
using SafeOrbit.Exceptions.SerializableException;
using SafeOrbit.Memory;
#if !NETSTANDARD1_6
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif

namespace SafeOrbit.Exceptions
{
    /// <summary>
    ///     An exception to throw when memory injection is detected.
    /// </summary>
    /// <seealso cref="SafeOrbitException" />
    /// <seealso cref="SerializableExceptionBase" />
#if !NETSTANDARD1_6
    [Serializable]
#endif
    public class MemoryInjectionException : SafeOrbitException
    {
        public MemoryInjectionException(InjectionType injectionType, object injectedObject,
            DateTimeOffset injectionTime)
            : base($"¨The object is injected by {injectionType}")
        {
            InjectionType = injectionType;
            InjectedObject = injectedObject;
            DetectionTime = injectionTime;
        }

        public MemoryInjectionException(string message, Exception inner) : base(message, inner)
        {
        }

        public MemoryInjectionException(InjectionType injectionType, string message, Exception inner)
            : base(message, inner)
        {
            InjectionType = injectionType;
        }

        public MemoryInjectionException(Exception inner) : base(inner)
        {
        }

        public MemoryInjectionException(Exception inner, InjectionType injectionType) : base(inner)
        {
            InjectionType = injectionType;
        }

#if !NETSTANDARD1_6
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public MemoryInjectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif

        public InjectionType InjectionType { get; set; }
        public object InjectedObject { get; set; }
        public DateTimeOffset DetectionTime { get; set; }

#if !NETSTANDARD1_6
        protected override void ConfigureSerialize(ISerializationContext serializationContext)
        {
            serializationContext.Add(() => InjectionType);
            serializationContext.Add(() => InjectedObject);
            serializationContext.Add(() => DetectionTime);
            base.ConfigureSerialize(serializationContext);
        }
#endif
    }
}