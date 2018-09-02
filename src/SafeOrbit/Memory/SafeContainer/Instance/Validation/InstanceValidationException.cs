using System;
using SafeOrbit.Exceptions;

#if NETFRAMEWORK
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Validation
{
#if NETFRAMEWORK
    [Serializable]
#endif
    public class InstanceValidationException : SafeOrbitException
    {
        public InstanceValidationException(string message, Exception inner) : base(message, inner)
        {
        }

#if NETFRAMEWORK
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public InstanceValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}