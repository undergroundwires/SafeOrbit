//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.IO;
using System.Text;
using SafeOrbit.Memory.Serialization.SerializationServices.Advanced.Binary;
using SafeOrbit.Memory.Serialization.SerializationServices.Advanced.Serializing;
using SafeOrbit.Memory.Serialization.SerializationServices.Core.Binary;

namespace SafeOrbit.Memory.Serialization.SerializationServices.Advanced
{
    /// <summary>
    ///   Reads data which was stored with the BurstBinaryWriter
    /// </summary>
    internal sealed class BurstBinaryReader : IBinaryReader
    {
        private readonly Encoding _encoding;
        private readonly ITypeNameConverter _typeNameConverter;
        private BinaryReader _reader;

        ///<summary>
        ///</summary>
        ///<param name = "typeNameConverter"></param>
        ///<param name = "encoding"></param>
        ///<exception cref = "ArgumentNullException"></exception>
        public BurstBinaryReader(ITypeNameConverter typeNameConverter, Encoding encoding)
        {
            if (typeNameConverter == null) throw new ArgumentNullException(nameof(typeNameConverter));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            _typeNameConverter = typeNameConverter;
            _encoding = encoding;
        }

        #region IBinaryReader Members

        /// <summary>
        ///   Reads property name
        /// </summary>
        /// <returns></returns>
        public string ReadName()
        {
            return BinaryReaderTools.ReadString(_reader);
        }

        /// <summary>
        ///   Reads single byte
        /// </summary>
        /// <returns></returns>
        public byte ReadElementId()
        {
            return _reader.ReadByte();
        }

        /// <summary>
        ///   Read type
        /// </summary>
        /// <returns></returns>
        public Type ReadType()
        {
            if (!_reader.ReadBoolean()) return null;
            var typeAsName = _reader.ReadString();
            return _typeNameConverter.ConvertToType(typeAsName);
        }

        /// <summary>
        ///   Read integer which was saved as 1,2 or 4 bytes, according to its size
        /// </summary>
        /// <returns></returns>
        public int ReadNumber()
        {
            return BinaryReaderTools.ReadNumber(_reader);
        }


        /// <summary>
        ///   Read array of integers which were saved as 1,2 or 4 bytes, according to their size
        /// </summary>
        /// <returns>Empty array if there are no indexes</returns>
        public int[] ReadNumbers()
        {
            return BinaryReaderTools.ReadNumbers(_reader);
        }

        /// <summary>
        ///   Reads simple value (value of a simple property)
        /// </summary>
        /// <param name = "expectedType"></param>
        /// <returns></returns>
        public object ReadValue(Type expectedType)
        {
            return BinaryReaderTools.ReadValue(expectedType, _reader);
        }

        /// <summary>
        ///   Opens the stream for reading
        /// </summary>
        /// <param name = "stream"></param>
        public void Open(Stream stream)
        {
            _reader = new BinaryReader(stream, _encoding);
        }

        /// <summary>
        ///   Does nothing, the stream can be further used and has to be manually closed
        /// </summary>
        public void Close()
        {
            // don't close the stream if you want further read it
            //_reader.Close();
        }
        #endregion
    }
}