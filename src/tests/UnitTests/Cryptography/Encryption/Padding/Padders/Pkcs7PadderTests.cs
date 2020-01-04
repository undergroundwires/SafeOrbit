using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using NUnit.Framework;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Tests;

namespace SafeOrbit.Cryptography.Encryption.Padding.Padders
{
    [TestFixture]
    public class Pkcs7PadderTests : TestsFor<IPkcs7Padder>
    {
        protected override IPkcs7Padder GetSut() => new Pkcs7Padder();

        [Test]
        public void Pad_DataIsNull_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            // Act
            void Action() => sut.Pad(null, 16);
            // Assert
            Assert.Throws<ArgumentNullException>(Action);
        }

        [Test]
        [TestCase(256), TestCase(0), TestCase(-1), TestCase(-100)]
        public void Pad_InvalidPaddingSize_ThrowsException(int invalidPaddingSize)
        {
            // Arrange
            var sut = GetSut();
            // Act
            void Action() => sut.Pad(new byte[32], invalidPaddingSize);
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Action);
        }

        [Test]
        public void Unpad_DataIsNull_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            // Act
            void Action() => sut.Unpad(null);
            // Assert
            Assert.Throws<ArgumentNullException>(Action);
        }

        [Test]
        public void Unpad_DataIsEmpty_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            // Act
            void Action() => sut.Unpad(new byte[0]);
            // Assert
            Assert.Throws<ArgumentException>(Action);
        }

        [Test]
        public void Unpad_DataHasSingleByte_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            // Act
            void Action() => sut.Unpad(new byte[1]);
            // Assert
            Assert.Throws<CryptographicException>(Action);
        }

        [Test]
        public void Unpad_TotalPaddedIsZero_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            var data = new byte[]{
                /* data: */ 1, 2, 3, 4, 5,
                /* padded: */ 0};
            // Act
            void Action() => sut.Unpad(data);
            // Assert
            Assert.Throws<CryptographicException>(Action);
        }

        [Test]
        public void Unpad_TotalPaddedIsBiggerThanData_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            var data = new byte[]{
                /* data: */ 1, 2, 3, 4, 5,
                /* padded: */ 6, 6, 6, 6, 6 };
            // Act
            void Action() => sut.Unpad(data);
            // Assert
            Assert.Throws<CryptographicException>(Action);
        }

        [Test]
        public void Unpad_BytesDoNotRepeatInPaddedPart_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            var data = new byte[]{
                /* data: */ 1, 2, 3, 4, 5,
                /* padded: */ 4, 4, 3, 4 };
            // Act
            void Action() => sut.Unpad(data);
            // Assert
            Assert.Throws<CryptographicException>(Action);
        }

        [Test]
        public void Unpad_ValidPaddedBytes_ReturnsExpected()
        {
            // Arrange
            var sut = GetSut();
            var data = new byte[] {1, 2, 3, 4, 5};
            var padded = data.Concat(new byte[]{4, 4, 4, 4 }).ToArray();
            // Act
            var actual = sut.Unpad(padded);
            // Assert
            CollectionAssert.AreEqual(data, actual);
        }

        [Test]
        [TestCaseSource(typeof(Pkcs7PadderTests), nameof(Pkcs7TestVector))]
        public void Padder_ValidLengthAndData_CanPadAndUnpad(int paddingLengthInBytes, byte[] rawBytes, byte[] expectedPadded)
        {
            var sut = GetSut();
            var paddedBytes = sut.Pad(rawBytes, paddingLengthInBytes);
            var unpaddedBytes = sut.Unpad(paddedBytes);
            CollectionAssert.AreEqual(expectedPadded, paddedBytes);
            CollectionAssert.AreEqual(unpaddedBytes, rawBytes);
        }

        [Test]
        [TestCase(16, 8), TestCase(5000, 255), TestCase(5000, 8), TestCase(32, 3), TestCase(255, 31), TestCase(0, 5)]
        public void Padder_DifferentValidLengths_CanPadAndUnpad(int bytesLength, int paddingLength)
        {
            var sut = GetSut();
            var rawBytes = FastRandom.StaticInstance.GetBytes(bytesLength);
            var paddedBytes = sut.Pad(rawBytes, paddingLength);
            var unpaddedBytes = sut.Unpad(paddedBytes);
            CollectionAssert.AreEqual(rawBytes, unpaddedBytes);
        }

        private static IEnumerable<object> Pkcs7TestVector
        {
            get
            {
                yield return GetTestCase(32, new byte[]
                {
                    66, 17, 81, 164, 89, 250, 234, 222,
                    61, 36, 113, 21, 249, 74, 237, 174,
                    66, 49, 129, 36, 9, 90, 250, 190,
                    77, 20, 81, 165, 89, 250, 237, 238
                }, new byte[]
                {
                    66, 17, 81, 164, 89, 250, 234, 222,
                    61, 36, 113, 21, 249, 74, 237, 174,
                    66, 49, 129, 36, 9, 90, 250, 190,
                    77, 20, 81, 165, 89, 250, 237, 238,
                    32, 32, 32, 32, 32, 32, 32, 32,
                    32, 32, 32, 32, 32, 32, 32, 32,
                    32, 32, 32, 32, 32, 32, 32, 32,
                    32, 32, 32, 32, 32, 32, 32, 32
                });
                yield return GetTestCase(2, new byte[]
                {
                    66, 17, 81, 164, 89, 250, 234, 222,
                    61, 36, 113, 21, 249, 74
                }, new byte[]
                {
                    66, 17, 81, 164, 89, 250, 234, 222,
                    61, 36, 113, 21, 249, 74, 02, 02
                });

                yield return GetTestCase(1, new byte[]
                {
                    66, 17, 81, 164,
                    89, 250, 234,
                }, new byte[]
                {
                    66, 17, 81, 164,
                    89, 250, 234, 1
                });

                object[] GetTestCase(int paddingLengthInBytes, byte[] original, byte[] expected)
                {
                    return new object[]  { paddingLengthInBytes, original, expected};
                }
            }
        }
    }
}
