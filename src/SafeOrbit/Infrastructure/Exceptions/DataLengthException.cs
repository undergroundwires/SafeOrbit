#if !NETCORE
using System.Runtime.Serialization;
using System;
using System.Security.Permissions;
#endif

namespace SafeOrbit.Exceptions
{
    /// <summary>
    ///     This exception is thrown if a buffer that is meant to have output copied into it turns out to be too short, or if
    ///     insufficient input was given..
    /// </summary>
#if !NETCORE
  [Serializable]
#endif
    public class DataLengthException : SafeOrbitException
    {
        public DataLengthException(string argumentName, string message) : base(argumentName, message)
        {
        }

#if !NETCORE
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public DataLengthException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}