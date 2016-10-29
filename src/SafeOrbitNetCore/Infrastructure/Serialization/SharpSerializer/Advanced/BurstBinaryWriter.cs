
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

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
using System.IO;
using System.Text;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced.Binary;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced.Serializing;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Core.Binary;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced
{
    /// <summary>
    ///     Stores data in a binary format. All types and property names which describe an object are stored together with the
    ///     object.
    ///     If there are more objects to store, their types are multiple stored, what increases the file size.
    ///     This format is simple and has small overhead.
    /// </summary>
    internal sealed class BurstBinaryWriter : IBinaryWriter
    {
        private readonly Encoding _encoding;
        private readonly ITypeNameConverter _typeNameConverter;
        private BinaryWriter _writer;

        /// <summary>
        /// </summary>
        /// <param name="typeNameConverter"></param>
        /// <param name="encoding"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BurstBinaryWriter(ITypeNameConverter typeNameConverter, Encoding encoding)
        {
            if (typeNameConverter == null) throw new ArgumentNullException(nameof(typeNameConverter));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            _encoding = encoding;
            _typeNameConverter = typeNameConverter;
        }

        #region IBinaryWriter Members

        /// <summary>
        ///     Writes Element Id
        /// </summary>
        /// <param name="id"></param>
        public void WriteElementId(byte id)
        {
            _writer.Write(id);
        }

        /// <summary>
        ///     Writes an integer. It saves the number with the least required bytes
        /// </summary>
        /// <param name="number"></param>
        public void WriteNumber(int number)
        {
            BinaryWriterTools.WriteNumber(number, _writer);
        }

        /// <summary>
        ///     Writes an array of numbers. It saves numbers with the least required bytes
        /// </summary>
        /// <param name="numbers"></param>
        public void WriteNumbers(int[] numbers)
        {
            BinaryWriterTools.WriteNumbers(numbers, _writer);
        }

        /// <summary>
        ///     Writes type
        /// </summary>
        /// <param name="type"></param>
        public void WriteType(Type type)
        {
            if (type == null)
            {
                _writer.Write(false);
            }
            else
            {
                _writer.Write(true);
                _writer.Write(_typeNameConverter.ConvertToTypeName(type));
            }
        }

        /// <summary>
        ///     Writes property name
        /// </summary>
        /// <param name="name"></param>
        public void WriteName(string name)
        {
            BinaryWriterTools.WriteString(name, _writer);
        }

        /// <summary>
        ///     Writes a simple value (value of a simple property)
        /// </summary>
        /// <param name="value"></param>
        public void WriteValue(object value)
        {
            BinaryWriterTools.WriteValue(value, _writer);
        }

        /// <summary>
        ///     Opens the stream for writing
        /// </summary>
        /// <param name="stream"></param>
        public void Open(Stream stream)
        {
            _writer = new BinaryWriter(stream, _encoding);
        }

        /// <summary>
        ///     Saves the data to the stream, the stream is not closed and can be further used
        /// </summary>
        public void Close()
        {
            _writer.Flush();
        }

        #endregion
    }
}