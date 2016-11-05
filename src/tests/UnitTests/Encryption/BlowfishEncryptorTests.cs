
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
using System.Collections;
using NUnit.Framework;
using SafeOrbit.Encryption;
using SafeOrbit.Exceptions;
using SafeOrbit.Tests.Cases;
using SafeOrbit.Tests;
using SafeOrbit.UnitTests;

namespace SafeOrbit.Cryptography.Encryption
{
    /// <seealso cref="IFastEncryptor" />
    /// <seealso cref="BlowfishEncryptor" />
    [TestFixture]
    public class BlowfishEncryptorTests : TestsFor<IFastEncryptor>
    {
        protected override IFastEncryptor GetSut() => new BlowfishEncryptor();
        private IFastEncryptor GetSut(BlowfishCipherMode cipherMode) => new BlowfishEncryptor(cipherMode);
        #region Parameter checks
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_InputParameterIsNull_throwsArgumentNullException(byte[] key)
        {
            //Arrange
            var sut = GetSut();
            var input = (byte[])null;
            //Act
            TestDelegate callingWithNullParameter = () => sut.Encrypt(input, key);
            //Assert
            Assert.That(callingWithNullParameter, Throws.TypeOf<ArgumentNullException>());
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
        public void Encrypt_KeyParameterIsNull_throwsArgumentNullException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = (byte[])null;
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
        public void Decrypt_InputParameterIsNull_throwsArgumentNullException(byte[] key)
        {
            //Arrange
            var sut = GetSut();
            var input = (byte[])null;
            //Act
            TestDelegate callingWithNullParameter = () => sut.Decrypt(input, key);
            //Assert
            Assert.That(callingWithNullParameter, Throws.TypeOf<ArgumentNullException>());
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
        public void Decrypt_KeyParameterIsNull_throwsArgumentNullException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = (byte[])null;
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

        #region [KeySize]
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeySizeIsLowerThanMinKey_throwsKeySizeException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[(sut.MinKeySize / 8) - 1];
            //Act
            TestDelegate callingWithWrongKeySize = () => sut.Encrypt(input, key);
            //Assert
            Assert.That(callingWithWrongKeySize, Throws.TypeOf<KeySizeException>());
        }
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Encrypt_KeySizeIsHigherThanMaxKey_throwsKeySizeException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[(sut.MaxKeySize / 8) + 1];
            //Act
            TestDelegate callingWithWrongKeySize = () => sut.Encrypt(input, key);
            //Assert
            Assert.That(callingWithWrongKeySize, Throws.TypeOf<KeySizeException>());
        }
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_KeySizeIsLowerThanMinKey_throwsKeySizeException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[(sut.MinKeySize / 8) - 1];
            //Act
            TestDelegate callingWithWrongKeySize = () => sut.Decrypt(input, key);
            //Assert
            Assert.That(callingWithWrongKeySize, Throws.TypeOf<KeySizeException>());
        }
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Decrypt_KeySizeIsHigherThanMaxKey_throwsKeySizeException(byte[] input)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[(sut.MaxKeySize / 8) + 1];
            //Act
            TestDelegate callingWithWrongKeySize = () => sut.Decrypt(input, key);
            //Assert
            Assert.That(callingWithWrongKeySize, Throws.TypeOf<KeySizeException>());
        }
        #endregion

        #endregion


        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Cbc_Decrypt_Encrypt_KeySizeIsBetweenMinKeyAndLastKey(byte[] input)
        {
            //arrange
            var sut = GetSut(BlowfishCipherMode.Cbc);
            for (var i = sut.MinKeySize / 8; i < sut.MaxKeySize / 8; i++)
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
        public void Ecb_Decrypt_Encrypt_KeySizeIsBetweenMinKeyAndLastKey(byte[] input)
        {
            //arrange
            var sut = GetSut(BlowfishCipherMode.Ecb);
            for (var i = sut.MinKeySize / 8; i < sut.MaxKeySize / 8; i++)
            {
                //act
                var key = new byte[i];
                var encrypted = sut.Encrypt(input, key);
                var plain = sut.Decrypt(encrypted, key);
                //assert
                Assert.That(input, Is.EqualTo(plain));
            }
        }

    }
}