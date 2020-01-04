//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
#if !NETSTANDARD1_6
using System.Runtime.Serialization;
#endif

namespace SafeOrbit.Core.Serialization.SerializationServices.Core
{
    /// <summary>
    ///     Can occur during deserialization
    /// </summary>
#if !NETSTANDARD1_6
    [Serializable]
#endif
    internal class DeserializingException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        ///<summary>
        ///</summary>
        public DeserializingException()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        public DeserializingException(string message) : base(message)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public DeserializingException(string message, Exception inner) : base(message, inner)
        {
        }

#if !NETSTANDARD1_6
        /// <summary>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected DeserializingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}