//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SafeOrbit.Memory.Serialization.SerializationServices.Advanced.Binary;
using SafeOrbit.Memory.Serialization.SerializationServices.Advanced.Serializing;
using SafeOrbit.Memory.Serialization.SerializationServices.Core.Binary;

namespace SafeOrbit.Memory.Serialization.SerializationServices.Advanced
{
    /// <summary>
    ///   Reads data which was stored with SizeOptimizedBinaryWriter
    /// </summary>
    internal sealed class SizeOptimizedBinaryReader : IBinaryReader
    {
        // .NET 2.0 doesn't support Func, it has to be manually declared
        private delegate T HeaderCallback<out T>(string text);

        private readonly Encoding _encoding;
        private readonly IList<string> _names = new List<string>();
        private readonly ITypeNameConverter _typeNameConverter;

        // Translation table of types
        private readonly IList<Type> _types = new List<Type>();
        private BinaryReader _reader;

        // Translation table of property names


        ///<summary>
        ///</summary>
        ///<param name = "typeNameConverter"></param>
        ///<param name = "encoding"></param>
        ///<exception cref = "ArgumentNullException"></exception>
        public SizeOptimizedBinaryReader(ITypeNameConverter typeNameConverter, Encoding encoding)
        {
            if (typeNameConverter == null) throw new ArgumentNullException(nameof(typeNameConverter));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            _typeNameConverter = typeNameConverter;
            _encoding = encoding;
        }

        #region IBinaryReader Members

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
            var index = BinaryReaderTools.ReadNumber(_reader);
            return _types[index];
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
        /// <returns></returns>
        public int[] ReadNumbers()
        {
            return BinaryReaderTools.ReadNumbers(_reader);
        }

        /// <summary>
        ///   Reads property name
        /// </summary>
        /// <returns></returns>
        public string ReadName()
        {
            var index = BinaryReaderTools.ReadNumber(_reader);
            return _names[index];
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

            // read names
            _names.Clear();
            ReadHeader(_reader, _names, text => text);

            // read types
            _types.Clear();
            ReadHeader(_reader, _types, _typeNameConverter.ConvertToType);
        }

        /// <summary>
        ///   Does nothing, the stream can be further used and has to be manually closed
        /// </summary>
        public void Close()
        {
            // nothing to do
        }

        #endregion

        private static void ReadHeader<T>(BinaryReader reader, ICollection<T> items, HeaderCallback<T> readCallback)
        {
            // Count
            var count = BinaryReaderTools.ReadNumber(reader);

            // Items
            for (var i = 0; i < count; i++)
            {
                var itemAsText = BinaryReaderTools.ReadString(reader);
                var item = readCallback(itemAsText);
                items.Add(item);
            }
        }
    }
}