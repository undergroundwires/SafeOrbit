
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced.Binary;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced.Serializing;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Core.Binary;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced
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
            _typeNameConverter = typeNameConverter ?? throw new ArgumentNullException(nameof(typeNameConverter));
            _encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
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