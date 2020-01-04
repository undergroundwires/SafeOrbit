//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.Runtime.Serialization;

namespace SafeOrbit.Core.Serialization.SerializationServices.Core
{
    /// <summary>
    ///     Occurs if the simple value can not be restored from its text representation
    /// </summary>
#if !NETSTANDARD1_6
    [Serializable]
#endif
    internal class SimpleValueParsingException : Exception
    {
        ///<summary>
        ///</summary>
        public SimpleValueParsingException()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        public SimpleValueParsingException(string message) : base(message)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public SimpleValueParsingException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if !NETSTANDARD1_6
        /// <summary>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SimpleValueParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}