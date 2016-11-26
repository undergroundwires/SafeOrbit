
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
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SafeOrbit.Cryptography.Encryption.Kdf;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Exceptions;
using SafeOrbit.Fakes;
using SafeOrbit.Tests;
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
        public void Encrypt_SameSalt_returnsDifferentResults(byte[] input, byte[] key, byte[] salt)
        {
            //Arrange
            var ivSize = 16;
            var randomMock = new Mock<IFastRandom>();
            randomMock.SetupSequence(r => r.GetBytes(It.Is<int>(i => i.Equals(ivSize))))
                .Returns(new byte[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0})
                .Returns(new byte[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1})
                .Returns(new byte[] {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2});
            var sut = GetSut(random: randomMock.Object);
            //Act
            var result1 = sut.Encrypt(input, key, salt);
            var result2 = sut.Encrypt(input, key, salt);
            var result3 = sut.Encrypt(input, key, salt);
            //Assert
            Assert.That(result1, Is.Not.EqualTo(result2));
            Assert.That(result1, Is.Not.EqualTo(result3));
            Assert.That(result2, Is.Not.EqualTo(result3));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Decrypt_InputParameterIsEmpty_throwsArgumentNullException(byte[] key, byte[] salt)
        {
            //Arrange
            var sut = GetSut();
            var input = new byte[0];
            //Act
            TestDelegate callingWithEmptyParameter = () => sut.Decrypt(input, key, salt);
            //Assert
            Assert.That(callingWithEmptyParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Decrypt_InputParameterIsNull_throwsArgumentNullException(byte[] key, byte[] salt)
        {
            //Arrange
            var sut = GetSut();
            var input = (byte[]) null;
            //Act
            TestDelegate callingWithNullParameter = () => sut.Decrypt(input, key, salt);
            //Assert
            Assert.That(callingWithNullParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Decrypt_KeyParameterIsEmpty_throwsArgumentNullException(byte[] input, byte[] salt)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[0];
            //Act
            TestDelegate callingWithEmptyParameter = () => sut.Decrypt(input, key, salt);
            //Assert
            Assert.That(callingWithEmptyParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Decrypt_KeyParameterIsNull_throwsArgumentNullException(byte[] input, byte[] salt)
        {
            //Arrange
            var sut = GetSut();
            var key = (byte[]) null;
            //Act
            TestDelegate callingWithEmptyParameter = () => sut.Decrypt(input, key, salt);
            //Assert
            Assert.That(callingWithEmptyParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Decrypt_SaltParameterIsEmpty_throwsArgumentNullException(byte[] input, byte[] key)
        {
            //Arrange
            var sut = GetSut();
            var salt = new byte[0];
            //Act
            TestDelegate callingWithEmptyParameter = () => sut.Decrypt(input, key, salt);
            //Assert
            Assert.That(callingWithEmptyParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Decrypt_SaltParameterIsNull_throwsArgumentNullException(byte[] input, byte[] key)
        {
            //Arrange
            var sut = GetSut();
            var salt = (byte[]) null;
            //Act
            TestDelegate callingWithEmptyParameter = () => sut.Decrypt(input, key, salt);
            //Assert
            Assert.That(callingWithEmptyParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Encrypt_InputParameterIsEmpty_throwsArgumentNullException(byte[] key, byte[] salt)
        {
            //Arrange
            var sut = GetSut();
            var input = new byte[0];
            //Act
            TestDelegate callingWithEmptyParameter = () => sut.Encrypt(input, key, salt);
            //Assert
            Assert.That(callingWithEmptyParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Encrypt_InputParameterIsNull_throwsArgumentNullException(byte[] key, byte[] salt)
        {
            //Arrange
            var sut = GetSut();
            var input = (byte[]) null;
            //Act
            TestDelegate callingWithNullParameter = () => sut.Encrypt(input, key, salt);
            //Assert
            Assert.That(callingWithNullParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Encrypt_KeyParameterIsEmpty_throwsKeySizeException(byte[] input, byte[] salt)
        {
            //Arrange
            var sut = GetSut();
            var key = new byte[0];
            //Act
            TestDelegate callingWithEmptyParameter = () => sut.Encrypt(input, key, salt);
            //Assert
            Assert.That(callingWithEmptyParameter, Throws.TypeOf<KeySizeException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Encrypt_KeyParameterIsNull_throwsArgumentNullException(byte[] input, byte[] salt)
        {
            //Arrange
            var sut = GetSut();
            var key = (byte[]) null;
            //Act
            TestDelegate callingWithNullParameter = () => sut.Encrypt(input, key, salt);
            //Assert
            Assert.That(callingWithNullParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Encrypt_SaltParameterIsEmpty_throwsArgumentNullException(byte[] input, byte[] key)
        {
            //Arrange
            var sut = GetSut();
            var salt = new byte[0];
            //Act
            TestDelegate callingWithEmptyParameter = () => sut.Encrypt(input, key, salt);
            //Assert
            Assert.That(callingWithEmptyParameter, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Encrypt_SaltParameterIsNull_throwsArgumentNullException(byte[] input, byte[] key)
        {
            //Arrange
            var sut = GetSut();
            var salt = (byte[]) null;
            //Act
            TestDelegate callingWithNullParameter = () => sut.Encrypt(input, key, salt);
            //Assert
            Assert.That(callingWithNullParameter, Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Encrypt_Decrypt_KeySizeIsMinKeySize(byte[] input, byte[] salt)
        {
            //arrange
            var sut = GetSut();
            var keySize = sut.MinKeySize/8;
                //act
                var key = new byte[keySize];
                var encrypted = sut.Encrypt(input, key, salt);
                var plain = sut.Decrypt(encrypted, key, salt);
                //assert
                Assert.That(input, Is.EqualTo(plain));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.TwoDifferentByteArrays16Length))]
        public void Encrypt_Decrypt_KeySizeIsMaxKeySize(byte[] input, byte[] salt)
        {
            //arrange
            var sut = GetSut();
            var keySize = sut.MaxKeySize / 8;
            //act
            var key = new byte[keySize];
            var encrypted = sut.Encrypt(input, key, salt);
            var plain = sut.Decrypt(encrypted, key, salt);
            //assert
            Assert.That(input, Is.EqualTo(plain));
        }
    }
}