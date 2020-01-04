//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.IO;

namespace SafeOrbit.Core.Serialization.SerializationServices.Advanced.Binary
{
    /// <summary>
    ///   Writes in a binary format
    /// </summary>
    internal interface IBinaryWriter
    {
        /// <summary>
        ///   Writes Element Id
        /// </summary>
        /// <param name = "id"></param>
        void WriteElementId(byte id);

        /// <summary>
        ///   Writes type
        /// </summary>
        /// <param name = "type"></param>
        void WriteType(Type type);

        /// <summary>
        ///   Writes property name
        /// </summary>
        /// <param name = "name"></param>
        void WriteName(string name);

        /// <summary>
        ///   Writes a simple value (value of a simple property)
        /// </summary>
        /// <param name = "value"></param>
        void WriteValue(object value);

        /// <summary>
        ///   Writes an integer. It saves the number with the least required bytes
        /// </summary>
        /// <param name = "number"></param>
        void WriteNumber(int number);

        /// <summary>
        ///   Writes an array of numbers. It saves numbers with the least required bytes
        /// </summary>
        /// <param name = "numbers"></param>
        void WriteNumbers(int[] numbers);

        /// <summary>
        ///   Opens the stream for writing
        /// </summary>
        /// <param name = "stream"></param>
        void Open(Stream stream);

        /// <summary>
        ///   Saves the data to the stream, the stream is not closed and can be further used
        /// </summary>
        void Close();
    }
}