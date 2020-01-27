//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.IO;

namespace SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices.Advanced.Binary
{
    /// <summary>
    ///     Reads from a binary format
    /// </summary>
    internal interface IBinaryReader
    {
        /// <summary>
        ///     Reads single byte
        /// </summary>
        /// <returns></returns>
        byte ReadElementId();

        /// <summary>
        ///     Read type
        /// </summary>
        /// <returns>null if no type defined</returns>
        Type ReadType();

        /// <summary>
        ///     Read integer which was saved as 1,2 or 4 bytes, according to its size
        /// </summary>
        /// <returns></returns>
        int ReadNumber();

        /// <summary>
        ///     Read array of integers which were saved as 1,2 or 4 bytes, according to their size
        /// </summary>
        /// <returns>empty array if no numbers defined</returns>
        int[] ReadNumbers();

        /// <summary>
        ///     Reads property name
        /// </summary>
        /// <returns>null if no name defined</returns>
        string ReadName();

        /// <summary>
        ///     Reads simple value (value of a simple property)
        /// </summary>
        /// <param name="expectedType"></param>
        /// <returns>null if no value defined</returns>
        object ReadValue(Type expectedType);

        /// <summary>
        ///     Opens the stream for reading
        /// </summary>
        /// <param name="stream"></param>
        void Open(Stream stream);

        /// <summary>
        ///     Does nothing, the stream can be further used and has to be manually closed
        /// </summary>
        void Close();
    }
}