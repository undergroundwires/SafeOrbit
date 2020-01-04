using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using SafeOrbit.Cryptography.Encryption.Padding;
using SafeOrbit.Cryptography.Encryption.Padding.Factory;
using SafeOrbit.Cryptography.Encryption.Padding.Padders;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Exceptions;
using SafeOrbit.Fakes;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Cryptography.Encryption
{
    /// <seealso cref="IFastEncryptor" />
    /// <seealso cref="BlowfishEncryptor" />
    [TestFixture]
    public class BlowfishEncryptorTests : PaddedEncryptorTestsBase
    {
        private static IFastEncryptor GetSut(
            BlowfishCipherMode? cipherMode = null,
            ICryptoRandom random = null,
            PaddingMode paddingMode = PaddingMode.PKCS7,
            IPadder padder = null)
        {
            IPadderFactory factory = null;
            if (padder != null)
            {
                var factoryMock = new Mock<IPadderFactory>();
                factoryMock.Setup(p => p.GetPadder(paddingMode))
                    .Returns(padder);
                factory = factoryMock.Object;
            }
            return new BlowfishEncryptor(cipherMode ?? BlowfishCipherMode.Cbc, paddingMode, random ?? Stubs.Get<IFastRandom>(), factory ?? Stubs.Get<IPadderFactory>());
        }


        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_InputParameterIsEmpty_ThrowsException(byte[] key)
        {
            //Arrange
            var sut = GetSut();
            var input = new byte[0];
            //Act
            void CallWithEmptyParameter() => sut.Decrypt(input, key);
            //Assert
            Assert.That(CallWithEmptyParameter, Throws.TypeOf<DataLengthException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_InputParameterIsNull_ThrowsArgumentNullException(byte[] key)
        {
            //Arrange
            var sut = GetSut();
            var input = (byte[])null;
            //Act
            void CallWithNullParameter() => sut.Decrypt(input, key);
            //Assert
            Assert.That(CallWithNullParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Decrypt_KeyParameterIsEmpty_ThrowsException()
        {
            //Arrange
            var sut = GetSut();
            var input = new byte[] { 5, 10, 15 };
            var key = new byte[0];
            //Act
            void CallWithEmptyParameter() => sut.Decrypt(input, key);
            //Assert
            Assert.That(CallWithEmptyParameter, Throws.TypeOf<KeySizeException>());
        }

        [Test]
        public void Decrypt_KeyParameterIsNull_ThrowsArgumentNullException()
        {
            //Arrange
            var input = new byte[] { 55 };
            var sut = GetSut();
            var key = (byte[])null;
            //Act
            void CallWithNullParameter() => sut.Decrypt(input, key);
            //Assert
            Assert.That(CallWithNullParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_KeySizeIsHigherThanMaxKey_ThrowsKeySizeException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[sut.MaxKeySizeInBits / 8 + 1];
            //Act
            void CallWithWrongKeySize() => sut.Decrypt(input, key);
            //Assert
            Assert.That(CallWithWrongKeySize, Throws.TypeOf<KeySizeException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_KeySizeIsLowerThanMinKey_ThrowsKeySizeException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[sut.MinKeySizeInBits / 8 - 1];
            //Act
            void CallingWithWrongKeySize() => sut.Decrypt(input, key);
            //Assert
            Assert.That(CallingWithWrongKeySize, Throws.TypeOf<KeySizeException>());
        }
        
        [Test]
        [TestCase(BlowfishCipherMode.Ecb), TestCase(BlowfishCipherMode.Cbc)]
        public void Decrypt_KeySizeIsBetweenMinKeyAndLastKey_CanDecrypt(BlowfishCipherMode cipherMode)
        {
            //arrange
            var input = (byte[])ByteCases.ByteArray32Length[0];
            var sut = GetSut(cipherMode);
            for (var i = sut.MinKeySizeInBits / 8; i < sut.MaxKeySizeInBits / 8; i++)
            {
                //act
                var key = new byte[i];
                var encrypted = sut.Encrypt(input, key);
                var plain = sut.Decrypt(encrypted, key);
                //assert
                Assert.That(input, Is.EqualTo(plain));
            }
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_InputParameterIsEmpty_ThrowsException(byte[] key)
        {
            //Arrange
            var sut = GetSut();
            var input = new byte[0];
            //Act
            void CallWithEmptyParameter() => sut.Decrypt(input, key);
            //Assert
            Assert.That(CallWithEmptyParameter, Throws.TypeOf<DataLengthException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_InputParameterIsNull_ThrowsArgumentNullException(byte[] key)
        {
            //Arrange
            var sut = GetSut();
            var input = (byte[])null;
            //Act
            void CallWithNullParameter() => sut.Encrypt(input, key);
            //Assert
            Assert.That(CallWithNullParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeyParameterIsEmpty_ThrowsArgumentNullException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[0];
            //Act
            void CallWithEmptyParameter() => sut.Encrypt(input, key);
            //Assert
            Assert.That(CallWithEmptyParameter, Throws.TypeOf<KeySizeException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeyParameterIsNull_ThrowsArgumentNullException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = (byte[])null;
            //Act
            void CallWithNullParameter() => sut.Encrypt(input, key);
            //Assert
            Assert.That(CallWithNullParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeySizeIsHigherThanMaxKey_ThrowsKeySizeException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[sut.MaxKeySizeInBits / 8 + 1];
            //Act
            void CallWithWrongKeySize() => sut.Encrypt(input, key);
            //Assert
            Assert.That(CallWithWrongKeySize, Throws.TypeOf<KeySizeException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeySizeIsLowerThanMinKey_ThrowsKeySizeException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[sut.MinKeySizeInBits / 8 - 1];
            //Act
            void CallWithWrongKeySize() => sut.Encrypt(input, key);
            //Assert
            Assert.That(CallWithWrongKeySize, Throws.TypeOf<KeySizeException>());
        }

        [Test]
        public void Encrypt_DataSizeIsBlockBytes_PadsAsExpected()
        {
            // Arrange
            var padder = new Mock<IPadder>();
            padder.Setup(s => s.Pad(It.IsAny<byte[]>(), It.IsAny<int>()))
                .Verifiable();
            var sut = GetSut(padder: padder.Object);
            var blockSizeInBytes = sut.BlockSizeInBits / 8;
            var data = new byte[blockSizeInBytes];

            // Act
            sut.Encrypt(data, new byte[sut.MinKeySizeInBits / 8]);

            // Assert
            padder.Verify(
                p=> p.Pad(
                    It.Is<byte[]>(input => input.SequenceEqual(data)), 
                    It.Is<int>(size => size == blockSizeInBytes))
                , Times.Once);
        }

        [Test]
        public void Encrypt_DataSizeIsLessThanBlockBytes_PadsAsExpected()
        {
            // Arrange
            var padder = new Mock<IPadder>();
            padder.Setup(s => s.Pad(It.IsAny<byte[]>(), It.IsAny<int>()))
                .Verifiable();
            var sut = GetSut(padder: padder.Object);
            var blockSizeInBytes = sut.BlockSizeInBits;
            const int expectedPadLength = 3;
            var data = new byte[blockSizeInBytes - expectedPadLength];

            // Act
            sut.Encrypt(data, new byte[sut.MinKeySizeInBits / 8]);

            // Assert
            padder.Verify(
                p => p.Pad(
                    It.Is<byte[]>(input => input.SequenceEqual(data)),
                    It.Is<int>(size => size == expectedPadLength))
                , Times.Once);
        }

        [Test]
        public void Encrypt_DataSizeIsHigherThanBlockBytes_PadsAsExpected()
        {
            // Arrange
            var padder = new Mock<IPadder>();
            padder.Setup(s => s.Pad(It.IsAny<byte[]>(), It.IsAny<int>()))
                .Verifiable();
            var sut = GetSut(padder: padder.Object);
            var blockSizeInBytes = sut.BlockSizeInBits / 8;
            var expectedPadLength = blockSizeInBytes - 5;
            var data = new byte[blockSizeInBytes + 5];

            // Act
            sut.Encrypt(data, new byte[sut.MinKeySizeInBits / 8]);

            // Assert
            padder.Verify(
                p => p.Pad(
                    It.Is<byte[]>(input => input.SequenceEqual(data)),
                    It.Is<int>(size => size == expectedPadLength))
                , Times.Once);
        }

        protected override IPaddedEncryptor GetEncryptor(PaddingMode mode)
            => GetSut(paddingMode: mode);
    }
}