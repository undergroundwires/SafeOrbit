using System;
using NUnit.Framework;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Exceptions;
using SafeOrbit.Fakes;
using SafeOrbit.Tests;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Cryptography.Encryption
{
    /// <seealso cref="IFastEncryptor" />
    /// <seealso cref="BlowfishEncryptor" />
    [TestFixture]
    public class BlowfishEncryptorTests
    {
        protected IFastEncryptor GetSut(ICryptoRandom random = null) => new BlowfishEncryptor(random ?? Stubs.Get<IFastRandom>());
        private IFastEncryptor GetSut(BlowfishCipherMode cipherMode) => new BlowfishEncryptor(cipherMode, Stubs.Get<IFastRandom>());


        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Cbc_Decrypt_Encrypt_KeySizeIsBetweenMinKeyAndLastKey(byte[] input)
        {
            //arrange
            var sut = GetSut(BlowfishCipherMode.Cbc);
            for (var i = sut.MinKeySize/8; i < sut.MaxKeySize/8; i++)
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
        public void Decrypt_InputParameterIsEmpty_throwsArgumentNullException(byte[] key)
        {
            //Arrange
            var sut = GetSut();
            var input = new byte[0];
            //Act
            TestDelegate callingWithEmptyParameter = () => sut.Decrypt(input, key);
            //Assert
            Assert.That(callingWithEmptyParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_InputParameterIsNull_throwsArgumentNullException(byte[] key)
        {
            //Arrange
            var sut = GetSut();
            var input = (byte[]) null;
            //Act
            TestDelegate callingWithNullParameter = () => sut.Decrypt(input, key);
            //Assert
            Assert.That(callingWithNullParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_KeyParameterIsEmpty_throwsArgumentNullException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[0];
            //Act
            TestDelegate callingWithEmptyParamater = () => sut.Decrypt(input, key);
            //Assert
            Assert.That(callingWithEmptyParamater, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_KeyParameterIsNull_throwsArgumentNullException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = (byte[]) null;
            //Act
            TestDelegate callingWithNullParameter = () => sut.Decrypt(input, key);
            //Assert
            Assert.That(callingWithNullParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_KeySizeIsHigherThanMaxKey_throwsKeySizeException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[sut.MaxKeySize/8 + 1];
            //Act
            TestDelegate callingWithWrongKeySize = () => sut.Decrypt(input, key);
            //Assert
            Assert.That(callingWithWrongKeySize, Throws.TypeOf<KeySizeException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_KeySizeIsLowerThanMinKey_throwsKeySizeException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[sut.MinKeySize/8 - 1];
            //Act
            TestDelegate callingWithWrongKeySize = () => sut.Decrypt(input, key);
            //Assert
            Assert.That(callingWithWrongKeySize, Throws.TypeOf<KeySizeException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Ecb_Decrypt_Encrypt_KeySizeIsBetweenMinKeyAndLastKey(byte[] input)
        {
            //arrange
            var sut = GetSut(BlowfishCipherMode.Ecb);
            for (var i = sut.MinKeySize/8; i < sut.MaxKeySize/8; i++)
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
        public void Encrypt_InputParameterIsEmpty_throwsArgumentNullException(byte[] key)
        {
            //Arrange
            var sut = GetSut();
            var input = new byte[0];
            //Act
            TestDelegate callingWithEmptyParameter = () => sut.Encrypt(input, key);
            //Assert
            Assert.That(callingWithEmptyParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_InputParameterIsNull_throwsArgumentNullException(byte[] key)
        {
            //Arrange
            var sut = GetSut();
            var input = (byte[]) null;
            //Act
            TestDelegate callingWithNullParameter = () => sut.Encrypt(input, key);
            //Assert
            Assert.That(callingWithNullParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeyParameterIsEmpty_throwsArgumentNullException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[0];
            //Act
            TestDelegate callingWithEmptyParameter = () => sut.Encrypt(input, key);
            //Assert
            Assert.That(callingWithEmptyParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeyParameterIsNull_throwsArgumentNullException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = (byte[]) null;
            //Act
            TestDelegate callingWithNullParameter = () => sut.Encrypt(input, key);
            //Assert
            Assert.That(callingWithNullParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeySizeIsHigherThanMaxKey_throwsKeySizeException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[sut.MaxKeySize/8 + 1];
            //Act
            TestDelegate callingWithWrongKeySize = () => sut.Encrypt(input, key);
            //Assert
            Assert.That(callingWithWrongKeySize, Throws.TypeOf<KeySizeException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeySizeIsLowerThanMinKey_throwsKeySizeException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[sut.MinKeySize/8 - 1];
            //Act
            TestDelegate callingWithWrongKeySize = () => sut.Encrypt(input, key);
            //Assert
            Assert.That(callingWithWrongKeySize, Throws.TypeOf<KeySizeException>());
        }
    }
}