﻿using System;
using System.IO;
using NUnit.Framework;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Core;
using SafeOrbit.Memory.SafeObject.SharpSerializer;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices
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

        private static void Serialize(object source, SafeOrbit.Infrastructure.Serialization.SerializationServices.SharpSerializer serializer, Func<byte[], byte[]> dataCallback)
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

        private SharpSerializer GetSut() => new SharpSerializer();
        private SharpSerializer GetSut(BinarySettings settings) => new SharpSerializer(settings);
    }
}