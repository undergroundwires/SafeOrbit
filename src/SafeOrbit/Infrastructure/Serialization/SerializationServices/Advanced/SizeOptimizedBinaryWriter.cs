
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
    ///   Stores data in a binary format. Data is stored in two steps. At first are all objects stored in a cache and all types are analyzed. 
    ///   Then all types and property names are sorted and placed in a list. Duplicates are removed. Serialized objects contain references
    ///   to these types and property names. It decreases file size, especially for serialization of collection (many items of the same type)
    ///   <see cref="SizeOptimizedBinaryWriter"/> has bigger overhead than <see cref="BurstBinaryWriter"/>
    /// </summary>
    /// <seealso cref="IBinaryWriter"/>
    /// <seealso cref="BurstBinaryWriter"/>
    internal sealed class SizeOptimizedBinaryWriter : IBinaryWriter
    {
        private readonly Encoding _encoding;
        private readonly ITypeNameConverter _typeNameConverter;
        private List<WriteCommand> _cache;
        private IndexGenerator<string> _names;
        private Stream _stream;
        private IndexGenerator<Type> _types;


        ///<summary>
        ///</summary>
        ///<param name = "typeNameConverter"></param>
        ///<param name = "encoding"></param>
        ///<exception cref = "ArgumentNullException"></exception>
        public SizeOptimizedBinaryWriter(ITypeNameConverter typeNameConverter, Encoding encoding)
        {
            if (typeNameConverter == null) throw new ArgumentNullException(nameof(typeNameConverter));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            _encoding = encoding;
            _typeNameConverter = typeNameConverter;
        }

        #region IBinaryWriter Members

        /// <summary>
        ///   Writes Property Id
        /// </summary>
        /// <param name = "id"></param>
        public void WriteElementId(byte id)
        {
            _cache.Add(new ByteWriteCommand(id));
        }

        /// <summary>
        ///   Writes type
        /// </summary>
        /// <param name = "type"></param>
        public void WriteType(Type type)
        {
            var typeIndex = _types.GetIndexOfItem(type);
            _cache.Add(new NumberWriteCommand(typeIndex));
        }

        /// <summary>
        ///   Writes property name
        /// </summary>
        /// <param name = "name"></param>
        public void WriteName(string name)
        {
            var nameIndex = _names.GetIndexOfItem(name);
            _cache.Add(new NumberWriteCommand(nameIndex));
        }

        /// <summary>
        ///   Writes a simple value (value of a simple property)
        /// </summary>
        /// <param name = "value"></param>
        public void WriteValue(object value)
        {
            _cache.Add(new ValueWriteCommand(value));
        }

        /// <summary>
        ///   Writes an integer. It saves the number with the least required bytes
        /// </summary>
        /// <param name = "number"></param>
        public void WriteNumber(int number)
        {
            _cache.Add(new NumberWriteCommand(number));
        }

        /// <summary>
        ///   Writes an array of numbers. It saves numbers with the least required bytes
        /// </summary>
        /// <param name = "numbers"></param>
        public void WriteNumbers(int[] numbers)
        {
            _cache.Add(new NumbersWriteCommand(numbers));
        }

        /// <summary>
        ///   Opens the stream for writing
        /// </summary>
        /// <param name = "stream"></param>
        public void Open(Stream stream)
        {
            _stream = stream;
            _cache = new List<WriteCommand>();
            _types = new IndexGenerator<Type>();
            _names = new IndexGenerator<string>();
        }


        /// <summary>
        ///   Saves the data to the stream, the stream is not closed and can be further used
        /// </summary>
        public void Close()
        {
            var writer = new BinaryWriter(_stream, _encoding);

            // Write Names
            WriteNamesHeader(writer);

            // Write Types
            WriteTypesHeader(writer);

            // Write Data
            WriteCache(_cache, writer);

            writer.Flush();
        }

        #endregion

        private static void WriteCache(IEnumerable<WriteCommand> cache, BinaryWriter writer)
        {
            foreach (var command in cache)
            {
                command.Write(writer);
            }
        }

        private void WriteNamesHeader(BinaryWriter writer)
        {
            // count
            BinaryWriterTools.WriteNumber(_names.Items.Count, writer);

            // Items
            foreach (var name in _names.Items)
            {
                BinaryWriterTools.WriteString(name, writer);
            }
        }

        private void WriteTypesHeader(BinaryWriter writer)
        {
            // count
            BinaryWriterTools.WriteNumber(_types.Items.Count, writer);

            // Items
            foreach (var type in _types.Items)
            {
                var typeName = _typeNameConverter.ConvertToTypeName(type);
                BinaryWriterTools.WriteString(typeName, writer);
            }
        }

        #region Nested type: ByteWriteCommand

        private sealed class ByteWriteCommand : WriteCommand
        {
            public ByteWriteCommand(byte data)
            {
                Data = data;
            }

            private byte Data { get; }

            public override void Write(BinaryWriter writer)
            {
                writer.Write(Data);
            }
        }

        #endregion

        #region Nested type: NumberWriteCommand

        private sealed class NumberWriteCommand : WriteCommand
        {
            public NumberWriteCommand(int data)
            {
                Data = data;
            }

            private int Data { get; }

            public override void Write(BinaryWriter writer)
            {
                BinaryWriterTools.WriteNumber(Data, writer);
            }
        }

        #endregion

        #region Nested type: NumbersWriteCommand

        private sealed class NumbersWriteCommand : WriteCommand
        {
            public NumbersWriteCommand(int[] data)
            {
                Data = data;
            }

            private int[] Data { get; }

            public override void Write(BinaryWriter writer)
            {
                BinaryWriterTools.WriteNumbers(Data, writer);
            }
        }

        #endregion

        #region Nested type: ValueWriteCommand

        private sealed class ValueWriteCommand : WriteCommand
        {
            public ValueWriteCommand(object data)
            {
                Data = data;
            }

            private object Data { get; }

            public override void Write(BinaryWriter writer)
            {
                BinaryWriterTools.WriteValue(Data, writer);
            }
        }

        #endregion

        #region Nested type: WriteCommand

        private abstract class WriteCommand
        {
            public abstract void Write(BinaryWriter writer);
        }

        #endregion
    }
}