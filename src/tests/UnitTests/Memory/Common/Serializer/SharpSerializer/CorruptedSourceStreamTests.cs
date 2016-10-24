
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

using System;
using System.IO;
using NUnit.Framework;
using SafeOrbit.Memory.Serialization.SerializationServices.Core;
using SafeOrbit.Memory.SafeObject.SharpSerializer;

namespace SafeOrbit.Memory.Serialization.SerializationServices
{
    [TestFixture]
    public class CorruptedSourceStreamTests
    {
        [Test]
        public void OptimizedBinaryStream_StreamIsCorrupted_throwsDeserializingException()
        {
            //arrange
            var sut = GetSut();
            var myArray = new[] { "ala", "ma", null, "kota" };
            //act
            TestDelegate action = () => Serialize(myArray, sut, ReplaceSomeBytesInData);
            //assert
            Assert.Throws<DeserializingException>(action);
        }

        [Test]
        public void BurstBinaryStream_StreamIsCorrupted_throwsDeserializingException()
        {
            //arrange
            var myArray = new[] { "ala", "ma", null, "kota" };
            var settings = new BinarySettings(BinarySerializationMode.Burst);
            var sut = GetSut(settings);
            //act
            TestDelegate action = () => Serialize(myArray, sut, ReplaceSomeBytesInData);
            //assert
            Assert.Throws<DeserializingException>(action);
        }


        [Test]
        public void OptimizedBinaryStream_SizeIsTooShort_throwsDeserializingException()
        {
            //arrange
            var myArray = new[] { "ala", "ma", null, "kota" };
            var settings = new BinarySettings(BinarySerializationMode.SizeOptimized);
            var sut = GetSut(settings);
            //act
            TestDelegate action = () => Serialize(myArray, sut,ShortenData);
            //assert
            Assert.Throws<DeserializingException>(action);
        }

        [Test]
        public void BurstBinaryStream_SizeIsTooShort_throwsDeserializingException()
        {
            //arrange
            var myArray = new[] { "ala", "ma", null, "kota" };
            var settings = new BinarySettings(BinarySerializationMode.Burst);
            var sut = GetSut(settings);
            //act
            TestDelegate action = () => Serialize(myArray, sut, ShortenData);
            //assert
            Assert.Throws<DeserializingException>(action);
        }

        private static void Serialize(object source, SafeOrbit.Memory.Serialization.SerializationServices.SharpSerializer serializer, Func<byte[], byte[]> dataCallback)
        {
            byte[] data;

            // Serializing
            using (var stream = new MemoryStream())
            {
                // serialize
                serializer.Serialize(source, stream);

                data = stream.ToArray();
            }


            // Modifying data
            if (dataCallback != null)
            {
                data = dataCallback(data);
            }

            // Deserializing
            using (var stream = new MemoryStream(data))
            {
                // deserialize
                var result = serializer.Deserialize(stream);

                // it comes never here
            }
        }

        private static byte[] ReplaceSomeBytesInData(byte[] data)
        {
            var startIndex = Convert.ToInt32(data.Length * 0.7);
            var endIndex = Convert.ToInt32(data.Length * 0.9);
            for (var i = startIndex; i <= endIndex; i++)
            {
                unchecked
                {
                    data[i] = Convert.ToByte(new System.Random().Next(255));
                }
            }
            return data;
        }

        private static byte[] ShortenData(byte[] data)
        {
            var result = new byte[Convert.ToInt32(data.Length * 0.9)];
            using (var stream = new MemoryStream(data))
            {
                stream.Read(result, 0, result.Length);
            }
            return result;
        }

        private SafeOrbit.Memory.Serialization.SerializationServices.SharpSerializer GetSut()
        {
            return new SafeOrbit.Memory.Serialization.SerializationServices.SharpSerializer();
        }
        private SafeOrbit.Memory.Serialization.SerializationServices.SharpSerializer GetSut(BinarySettings settings)
        {
            return new SafeOrbit.Memory.Serialization.SerializationServices.SharpSerializer(settings);
        }
    }
}