using System;
using SafeOrbit.Exceptions;

#if !NETSTANDARD1_6
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Validation
{
#if !NETSTANDARD1_6
    [Serializable]
#endif
    public class InstanceValidationException : SafeOrbitException
    {
        public InstanceValidationException(string message, Exception inner) : base(message, inner)
        {
        }

#if !NETSTANDARD1_6
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public InstanceValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}