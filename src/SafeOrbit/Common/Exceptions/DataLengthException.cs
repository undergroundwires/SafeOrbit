using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SafeOrbit.Exceptions
{
    /// <summary>
    ///     This exception is thrown if a buffer that is meant to have output copied into it turns out to be too short, or if
    ///     we've been given insufficient input.
    /// </summary>
    [Serializable]
    public class DataLengthException : SafeOrbitException
    {
        public DataLengthException(string argumentName, string message) : base(argumentName, message)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public DataLengthException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}