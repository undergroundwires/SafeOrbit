using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SafeOrbit.Exceptions
{
    [Serializable]
    public class ReadOnlyAccessForbiddenException : SafeOrbitException
    {
        public ReadOnlyAccessForbiddenException(string message) : base(message)
        {
        }
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public ReadOnlyAccessForbiddenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}