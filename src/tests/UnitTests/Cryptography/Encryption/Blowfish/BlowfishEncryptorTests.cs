using System;
using System.Linq;
using System.Threading.Tasks;
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

            return new BlowfishEncryptor(cipherMode ?? BlowfishCipherMode.Cbc, paddingMode,
                random ?? Stubs.Get<IFastRandom>(), factory ?? Stubs.Get<IPadderFactory>());
        }


        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_InputParameterIsEmpty_ThrowsException(byte[] key)
        {
            // Arrange
            var sut = GetSut();
            var input = new byte[0];

            // Act
            Task CallWithEmptyParameter() => sut.DecryptAsync(input, key);

            // Assert
            Assert.ThrowsAsync<DataLengthException>(CallWithEmptyParameter);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_InputParameterIsNull_ThrowsArgumentNullException(byte[] key)
        {
            // Arrange
            var sut = GetSut();
            var input = (byte[]) null;

            // Act
            Task CallWithNullParameter() => sut.DecryptAsync(input, key);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(CallWithNullParameter);
        }

        [Test]
        public void Decrypt_KeyParameterIsEmpty_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            var input = new byte[] {5, 10, 15};
            var key = new byte[0];

            // Act
            Task CallWithEmptyParameter() => sut.DecryptAsync(input, key);

            // Assert
            Assert.ThrowsAsync<KeySizeException>(CallWithEmptyParameter);
        }

        [Test]
        public void Decrypt_KeyParameterIsNull_ThrowsArgumentNullException()
        {
            //Arrange
            var input = new byte[] {55};
            var sut = GetSut();
            var key = (byte[]) null;

            // Act
            Task CallWithNullParameter() => sut.DecryptAsync(input, key);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(CallWithNullParameter);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_KeySizeIsHigherThanMaxKey_ThrowsKeySizeException(byte[] input)
        {
            // Arrange
            var sut = GetSut();
            var key = new byte[sut.MaxKeySizeInBits / 8 + 1];

            // Act
            Task CallWithWrongKeySize() => sut.DecryptAsync(input, key);

            // Assert
            Assert.ThrowsAsync<KeySizeException>(CallWithWrongKeySize);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_KeySizeIsLowerThanMinKey_ThrowsKeySizeException(byte[] input)
        {
            // Arrange
            var sut = GetSut();
            var key = new byte[sut.MinKeySizeInBits / 8 - 1];

            // Act
            Task CallingWithWrongKeySize() => sut.DecryptAsync(input, key);

            // Assert
            Assert.ThrowsAsync<KeySizeException>(CallingWithWrongKeySize);
        }

        [Test]
        [TestCase(BlowfishCipherMode.Ecb)]
        [TestCase(BlowfishCipherMode.Cbc)]
        public async Task Decrypt_KeySizeIsBetweenMinKeyAndLastKey_CanDecrypt(BlowfishCipherMode cipherMode)
        {
            // Arrange
            var input = (byte[]) ByteCases.ByteArray32Length[0];
            var sut = GetSut(cipherMode);
            for (var i = sut.MinKeySizeInBits / 8; i < sut.MaxKeySizeInBits / 8; i++)
            {
                //act
                var key = new byte[i];
                var encrypted = await sut.EncryptAsync(input, key);
                var plain = await sut.DecryptAsync(encrypted, key);
                //assert
                Assert.That(input, Is.EqualTo(plain));
            }
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_InputParameterIsEmpty_ThrowsException(byte[] key)
        {
            // Arrange
            var sut = GetSut();
            var input = new byte[0];

            // Act
            Task CallWithEmptyParameter() => sut.DecryptAsync(input, key);

            // Assert
            Assert.ThrowsAsync<DataLengthException>(CallWithEmptyParameter);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_InputParameterIsNull_ThrowsArgumentNullException(byte[] key)
        {
            // Arrange
            var sut = GetSut();
            var input = (byte[]) null;

            // Act
            Task CallWithNullParameter() => sut.EncryptAsync(input, key);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(CallWithNullParameter);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeyParameterIsEmpty_ThrowsArgumentNullException(byte[] input)
        {
            // Arrange
            var sut = GetSut();
            var key = new byte[0];

            // Act
            Task CallWithEmptyParameter() => sut.EncryptAsync(input, key);

            // Assert
            Assert.ThrowsAsync<KeySizeException>(CallWithEmptyParameter);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeyParameterIsNull_ThrowsArgumentNullException(byte[] input)
        {
            // Arrange
            var sut = GetSut();
            var key = (byte[]) null;

            // Act
            Task CallWithNullParameter() => sut.EncryptAsync(input, key);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(CallWithNullParameter);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeySizeIsHigherThanMaxKey_ThrowsKeySizeException(byte[] input)
        {
            // Arrange
            var sut = GetSut();
            var key = new byte[sut.MaxKeySizeInBits / 8 + 1];

            // Act
            Task CallWithWrongKeySize() => sut.EncryptAsync(input, key);

            // Assert
            Assert.ThrowsAsync<KeySizeException>(CallWithWrongKeySize);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeySizeIsLowerThanMinKey_ThrowsKeySizeException(byte[] input)
        {
            // Arrange
            var sut = GetSut();
            var key = new byte[sut.MinKeySizeInBits / 8 - 1];

            // Act
            Task CallWithWrongKeySize() => sut.EncryptAsync(input, key);

            //Assert
            Assert.ThrowsAsync<KeySizeException>(CallWithWrongKeySize);
        }

        [Test]
        public async Task Encrypt_DataSizeIsBlockBytes_PadsAsExpected()
        {
            // Arrange
            var padder = new Mock<IPadder>();
            padder.Setup(s => s.Pad(It.IsAny<byte[]>(), It.IsAny<int>()))
                .Verifiable();
            var sut = GetSut(padder: padder.Object);
            var blockSizeInBytes = sut.BlockSizeInBits / 8;
            var data = new byte[blockSizeInBytes];

            // Act
            await sut.EncryptAsync(data, new byte[sut.MinKeySizeInBits / 8]);

            // Assert
            padder.Verify(
                p => p.Pad(
                    It.Is<byte[]>(input => input.SequenceEqual(data)),
                    It.Is<int>(size => size == blockSizeInBytes))
                , Times.Once);
        }

        [Test]
        public async Task Encrypt_DataSizeIsLessThanBlockBytes_PadsAsExpected()
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
            await sut.EncryptAsync(data, new byte[sut.MinKeySizeInBits / 8]);

            // Assert
            padder.Verify(
                p => p.Pad(
                    It.Is<byte[]>(input => input.SequenceEqual(data)),
                    It.Is<int>(size => size == expectedPadLength))
                , Times.Once);
        }

        [Test]
        public async Task Encrypt_DataSizeIsHigherThanBlockBytes_PadsAsExpected()
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
            await sut.EncryptAsync(data, new byte[sut.MinKeySizeInBits / 8]);

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