﻿using System;
using System.Runtime.Serialization;
using SafeOrbit.Exceptions.SerializableException;

#if !NETCORE
using System.Security.Permissions;
#endif

namespace SafeOrbit.Exceptions
{
    /// <summary>
    ///     An abstract class for all of special exceptions that SafeOrbit throws.
    /// </summary>
    /// <seealso cref="SerializableExceptionBase"/>
#if !NETCORE
    [Serializable]
#endif
    public abstract class SafeOrbitException : SerializableExceptionBase
    {
        protected SafeOrbitException(string argumentName, string message)
            : base($"{message} [Argument Name={argumentName}]")
        {
        }

        protected SafeOrbitException(string message) : base(message)
        {
        }

        protected SafeOrbitException()
        {
        }

        protected SafeOrbitException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SafeOrbitException(Exception innerException) : base(innerException)
        {
        }

#if !NETCORE
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected SafeOrbitException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}