#if !NETCORE
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif

namespace SafeOrbit.Exceptions
{
    /// <summary>
    /// This type of exception is thrown when trying to modify an object with only read only access.
    /// </summary>
    /// <remarks>
    /// TODO: rename this class.
    /// </remarks>
#if !NETCORE
    [Serializable]
#endif
    public class ReadOnlyAccessForbiddenException : SafeOrbitException
    {
        public ReadOnlyAccessForbiddenException(string message) : base(message)
        {
        }

#if !NETCORE
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public ReadOnlyAccessForbiddenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}