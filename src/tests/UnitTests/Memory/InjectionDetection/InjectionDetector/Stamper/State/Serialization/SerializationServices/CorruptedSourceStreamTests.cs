using System;
using System.IO;
using NUnit.Framework;
using SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices.Core;

namespace SafeOrbit.Memory.InjectionServices.Stampers.Serialization.SerializationServices
{
    [TestFixture]
    public class CorruptedSourceStreamTests
    {
        [Test]
        public void OptimizedBinaryStream_StreamIsCorrupted_ThrowsDeserializingException()
        {
            // Arrange
            var sut = GetSut();
            var myArray = new[] {"ala", "ma", null, "kota"};

            // Act
            void Action() => Serialize(myArray, sut, ReplaceSomeBytesInData);

            // Assert
            Assert.Throws<DeserializingException>(Action);
        }

        [Test]
        public void BurstBinaryStream_StreamIsCorrupted_ThrowsDeserializingException()
        {
            // Arrange
            var myArray = new[] {"ala", "ma", null, "kota"};
            var settings = new BinarySettings(BinarySerializationMode.Burst);
            var sut = GetSut(settings);

            // Act
            void Action() => Serialize(myArray, sut, ReplaceSomeBytesInData);

            // Assert
            Assert.Throws<DeserializingException>(Action);
        }


        [Test]
        public void OptimizedBinaryStream_SizeIsTooShort_ThrowsDeserializingException()
        {
            // Arrange
            var myArray = new[] {"ala", "ma", null, "kota"};
            var settings = new BinarySettings(BinarySerializationMode.SizeOptimized);
            var sut = GetSut(settings);

            // Act
            void Action() => Serialize(myArray, sut, ShortenData);

            // Assert
            Assert.Throws<DeserializingException>(Action);
        }

        [Test]
        public void BurstBinaryStream_SizeIsTooShort_throwsDeserializingException()
        {
            // Arrange
            var myArray = new[] {"ala", "ma", null, "kota"};
            var settings = new BinarySettings(BinarySerializationMode.Burst);
            var sut = GetSut(settings);

            // Act
            void Action() => Serialize(myArray, sut, ShortenData);

            // Assert
            Assert.Throws<DeserializingException>(Action);
        }

        private static void Serialize(object source, SharpSerializer serializer, Func<byte[], byte[]> dataCallback)
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
            if (dataCallback != null) data = dataCallback(data);

            // Deserializing
            using (var stream = new MemoryStream(data))
            {
                // deserialize
                var unused = serializer.Deserialize(stream);

                // it comes never here
            }
        }

        private static byte[] ReplaceSomeBytesInData(byte[] data)
        {
            var startIndex = Convert.ToInt32(data.Length * 0.7);
            var endIndex = Convert.ToInt32(data.Length * 0.9);
            for (var i = startIndex; i <= endIndex; i++)
                unchecked
                {
                    data[i] = Convert.ToByte(new Random().Next(255));
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

        private static SharpSerializer GetSut() => new SharpSerializer();
        private static SharpSerializer GetSut(BinarySettings settings) => new SharpSerializer(settings);
    }
}