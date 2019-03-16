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
            void CallWithEmptyParameter() => sut.Decrypt(input, key);
            //Assert
            Assert.That(CallWithEmptyParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_InputParameterIsNull_throwsArgumentNullException(byte[] key)
        {
            //Arrange
            var sut = GetSut();
            var input = (byte[]) null;
            //Act
            void CallWithNullParameter() => sut.Decrypt(input, key);
            //Assert
            Assert.That(CallWithNullParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_KeyParameterIsEmpty_throwsArgumentNullException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[0];
            //Act
            void CallWithEmptyParamater() => sut.Decrypt(input, key);
            //Assert
            Assert.That(CallWithEmptyParamater, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_KeyParameterIsNull_throwsArgumentNullException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = (byte[]) null;
            //Act
            void CallWithNullParameter() => sut.Decrypt(input, key);
            //Assert
            Assert.That(CallWithNullParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_KeySizeIsHigherThanMaxKey_throwsKeySizeException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[sut.MaxKeySize/8 + 1];
            //Act
            void CallWithWrongKeySize() => sut.Decrypt(input, key);
            //Assert
            Assert.That(CallWithWrongKeySize, Throws.TypeOf<KeySizeException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_KeySizeIsLowerThanMinKey_throwsKeySizeException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[sut.MinKeySize/8 - 1];
            //Act
            void CallingWithWrongKeySize() => sut.Decrypt(input, key);
            //Assert
            Assert.That(CallingWithWrongKeySize, Throws.TypeOf<KeySizeException>());
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
            void CallWithEmptyParameter() => sut.Decrypt(input, key);
            //Assert
            Assert.That(CallWithEmptyParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_InputParameterIsNull_throwsArgumentNullException(byte[] key)
        {
            //Arrange
            var sut = GetSut();
            var input = (byte[]) null;
            //Act
            void CallWithNullParameter() => sut.Encrypt(input, key);
            //Assert
            Assert.That(CallWithNullParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeyParameterIsEmpty_throwsArgumentNullException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[0];
            //Act
            void CallWithEmptyParameter() => sut.Encrypt(input, key);
            //Assert
            Assert.That(CallWithEmptyParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeyParameterIsNull_throwsArgumentNullException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = (byte[]) null;
            //Act
            void CallWithNullParameter() => sut.Encrypt(input, key);
            //Assert
            Assert.That(CallWithNullParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeySizeIsHigherThanMaxKey_throwsKeySizeException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[sut.MaxKeySize/8 + 1];
            //Act
            void CallWithWrongKeySize() => sut.Encrypt(input, key);
            //Assert
            Assert.That(CallWithWrongKeySize, Throws.TypeOf<KeySizeException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeySizeIsLowerThanMinKey_throwsKeySizeException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[sut.MinKeySize/8 - 1];
            //Act
            void CallWithWrongKeySize() => sut.Encrypt(input, key);
            //Assert
            Assert.That(CallWithWrongKeySize, Throws.TypeOf<KeySizeException>());
        }
    }
}