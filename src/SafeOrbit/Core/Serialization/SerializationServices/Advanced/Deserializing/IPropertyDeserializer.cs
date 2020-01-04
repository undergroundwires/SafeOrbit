//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System.IO;
using SafeOrbit.Core.Serialization.SerializationServices.Core;

namespace SafeOrbit.Core.Serialization.SerializationServices.Advanced.Deserializing
{
    /// <summary>
    ///     Deserializes a stream and gives back a Property
    /// </summary>
    internal interface IPropertyDeserializer
    {
        /// <summary>
        ///     Open the stream to read
        /// </summary>
        /// <param name="stream"></param>
        void Open(Stream stream);

        /// <summary>
        ///     Reading the stream
        /// </summary>
        /// <returns></returns>
        Property Deserialize();

        /// <summary>
        ///     Cleans all
        /// </summary>
        void Close();
    }
}