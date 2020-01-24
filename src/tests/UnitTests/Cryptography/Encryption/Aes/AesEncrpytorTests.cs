using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SafeOrbit.Cryptography.Encryption.Kdf;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Exceptions;
using SafeOrbit.Fakes;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Cryptography.Encryption
{
    /// <seealso cref="ISafeEncryptor" />
    /// <seealso cref="AesEncryptor" />
    [TestFixture]
    public class AesEncryptorTests
    {
        protected ISafeEncryptor GetSut(IKeyDerivationFunction kdf = null, IFastRandom random = null)
        {
            return new AesEncryptor(kdf ?? Stubs.Get<IKeyDerivationFunction>(), random ?? Stubs.Get<IFastRandom>());
        }

        /// <summary>
        ///     Tests the iv
        /// </summary>
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ThreeDifferentByteArrays16Length))]
        public async Task Encrypt_SameSalt_returnsDifferentResults(byte[] input, byte[] key, byte[] salt)
        {
            // Arrange
            const int ivSize = 16;
            var randomMock = new Mock<IFastRandom>();
            randomMock.SetupSequence(r => r.GetBytes(It.Is<int>(i => i.Equals(ivSize))))
                .Returns(new byte[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0})
                .Returns(new byte[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1})
                .Returns(new byte[] {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2});
            var sut = GetSut(random: randomMock.Object);

            // Act
            var result1 = await sut.EncryptAsync(input, key, salt);
            var result2 = await sut.EncryptAsync(input, key, salt);
            var result3 = await sut.EncryptAsync(input, key, salt);

            // Assert
            Assert.That(result1, Is.Not.EqualTo(result2));
            Assert.That(result1, Is.Not.EqualTo(result3));
            Assert.That(result2, Is.Not.EqualTo(result3));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Decrypt_InputParameterIsEmpty_throwsArgumentNullException(byte[] key, byte[] salt)
        {
            // Arrange
            var sut = GetSut();
            var input = new byte[0];

            // Act
            Task CallWithEmptyParameter() => sut.DecryptAsync(input, key, salt);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(CallWithEmptyParameter);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Decrypt_InputParameterIsNull_throwsArgumentNullException(byte[] key, byte[] salt)
        {
            // Arrange
            var sut = GetSut();
            var input = (byte[]) null;

            // Act
            Task CallWithNullParameter() => sut.DecryptAsync(input, key, salt);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(CallWithNullParameter);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Decrypt_KeyParameterIsEmpty_throwsKeySizeException(byte[] input, byte[] salt)
        {
            // Arrange
            var sut = GetSut();
            var key = new byte[0];

            // Act
            Task CallWithEmptyParameter() => sut.DecryptAsync(input, key, salt);

            // Assert
            Assert.ThrowsAsync<KeySizeException>(CallWithEmptyParameter);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Decrypt_KeyParameterIsNull_throwsArgumentNullException(byte[] input, byte[] salt)
        {
            // Arrange
            var sut = GetSut();
            var key = (byte[]) null;

            // Act
            Task CallWithEmptyParameter() => sut.DecryptAsync(input, key, salt);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(CallWithEmptyParameter);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Decrypt_SaltParameterIsEmpty_throwsArgumentNullException(byte[] input, byte[] key)
        {
            // Arrange
            var sut = GetSut();
            var salt = new byte[0];

            // Act
            Task CallWithEmptyParameter() => sut.DecryptAsync(input, key, salt);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(CallWithEmptyParameter);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Decrypt_SaltParameterIsNull_throwsArgumentNullException(byte[] input, byte[] key)
        {
            // Arrange
            var sut = GetSut();
            var salt = (byte[]) null;

            // Act
            Task CallWithEmptyParameter() => sut.DecryptAsync(input, key, salt);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(CallWithEmptyParameter);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Encrypt_InputParameterIsEmpty_throwsArgumentNullException(byte[] key, byte[] salt)
        {
            // Arrange
            var sut = GetSut();
            var input = new byte[0];

            // Act
            Task CallWithEmptyParameter() => sut.EncryptAsync(input, key, salt);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(CallWithEmptyParameter);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Encrypt_InputParameterIsNull_throwsArgumentNullException(byte[] key, byte[] salt)
        {
            // Arrange
            var sut = GetSut();
            var input = (byte[]) null;

            // Act
            Task CallWithNullParameter() => sut.EncryptAsync(input, key, salt);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(CallWithNullParameter);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Encrypt_KeyParameterIsEmpty_throwsKeySizeException(byte[] input, byte[] salt)
        {
            // Arrange
            var sut = GetSut();
            var key = new byte[0];

            // Act
            Task CallWithEmptyParameter() => sut.EncryptAsync(input, key, salt);

            // Assert
            Assert.ThrowsAsync<KeySizeException>(CallWithEmptyParameter);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Encrypt_KeyParameterIsNull_throwsArgumentNullException(byte[] input, byte[] salt)
        {
            // Arrange
            var sut = GetSut();
            var key = (byte[]) null;

            // Act
            Task CallWithNullParameter() => sut.EncryptAsync(input, key, salt);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(CallWithNullParameter);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Encrypt_SaltParameterIsEmpty_throwsArgumentNullException(byte[] input, byte[] key)
        {
            // Arrange
            var sut = GetSut();
            var salt = new byte[0];

            // Act
            Task CallWithEmptyParameter() => sut.EncryptAsync(input, key, salt);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(CallWithEmptyParameter);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Encrypt_SaltParameterIsNull_throwsArgumentNullException(byte[] input, byte[] key)
        {
            // Arrange
            var sut = GetSut();
            var salt = (byte[]) null;

            // Act
            Task CallWithNullParameter() => sut.EncryptAsync(input, key, salt);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(CallWithNullParameter);
        }


        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public async Task Encrypt_Decrypt_KeySizeIsMinKeySize(byte[] input, byte[] salt)
        {
            // Arrange
            var sut = GetSut();
            var keySize = sut.MinKeySizeInBits / 8;

            // Act
            var key = new byte[keySize];
            var encrypted = await sut.EncryptAsync(input, key, salt);
            var plain = await sut.DecryptAsync(encrypted, key, salt);

            // Assert
            Assert.That(input, Is.EqualTo(plain));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public async Task Encrypt_Decrypt_KeySizeIsMaxKeySize(byte[] input, byte[] salt)
        {
            // Arrange
            var sut = GetSut();
            var keySize = sut.MaxKeySizeInBits / 8;

            // Act
            var key = new byte[keySize];
            var encrypted = await sut.EncryptAsync(input, key, salt);
            var plain = await sut.DecryptAsync(encrypted, key, salt);

            // Assert
            Assert.That(input, Is.EqualTo(plain));
        }

        [Test]
        public void BlockSize_Is_128()
        {
            // "Strictly speaking, the AES standard is a variant of Rijndael where the block size is restricted to 
            // 128 bits" From : https://en.wikipedia.org/wiki/Advanced_Encryption_Standard
            // So the block size is always 128.
            const int expected = 128;
            var sut = GetSut();
            var actual = sut.BlockSizeInBits;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(1), TestCase(15), TestCase(16), TestCase(17), TestCase(300)]  // Boundary values for block sizes
        public async Task Encrypt_ForDifferentSizes_CanDecrypt(int size) // a.k.a. padding works
        {
            // Arrange
            var expected = FastRandom.StaticInstance.GetBytes(size);
            var sut = GetSut();
            var key = new byte[sut.MinKeySizeInBits];
            var salt = new byte[32];

            // Act
            var encrypted = await sut.EncryptAsync(expected, key, salt);
            var actual = await sut.DecryptAsync(encrypted, key, salt);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}