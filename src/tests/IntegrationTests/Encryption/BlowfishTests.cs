
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

using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using SafeOrbit.Encryption;
using SafeOrbit.Memory;
using SafeOrbit.Tests.Cases;
using SafeOrbit.Tests;
using SafeOrbit.Vectors;

namespace SafeOrbit.IntegrationTests.Encryption
{
    [TestFixture]
    public class BlowfishTests : TestsFor<IFastEncryptor>
    {
        protected override IFastEncryptor GetSut()
        {
            return new BlowfishEncryptor();
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentByteArrayPairs32Length))]
        public void Encrypt_returnsDifferentValueThanInput(byte[] input, byte[] key)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var output = sut.Encrypt(input, key);
            //Assert
            Assert.IsFalse(input.SequenceEqual(output));
        }


        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ThreeDifferentByteArrays16Length))]
        public void Encrypt_WhenDifferentKeysGiven_returnsDifferentValues(byte[] input, byte[] key, byte[] differentKey)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var output = sut.Encrypt(input, key);
            var outputFromDifferentKey = sut.Encrypt(input, differentKey);
            //Assert
            Assert.IsFalse(output.SequenceEqual(outputFromDifferentKey));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentByteArrayPairs32Length))]
        public void Decrypt_Ecb_WhenDecryptedWithEncryptionKey_returnsInput(byte[] input, byte[] key)
        {
            //Arrange
            var sut = new BlowfishEncryptor(BlowfishCipherMode.Ecb); 
            var encrypted = sut.Encrypt(input, key);
            //Act
            var output = sut.Decrypt(encrypted, key);
            //Assert
            Assert.IsTrue(input.SequenceEqual(output));
        }
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentByteArrayPairs32Length))]
        public void Decrypt_Cbc_WhenDecryptedWithEncryptionKey_returnsInput(byte[] input, byte[] key)
        {
            //Arrange
            var sut = new BlowfishEncryptor(BlowfishCipherMode.Cbc);
            var encrypted = sut.Encrypt(input, key);
            //Act
            var output = sut.Decrypt(encrypted, key);
            //Assert
            Assert.IsTrue(input.SequenceEqual(output));
        }

        [Test]
        [TestCaseSource(typeof(BlowfishVectors), nameof(BlowfishVectors.Vectors))]
        public void Encrypt_Ecb_ReturnsVector(byte[] keyBytes, byte[] clearBytes, byte[] cipherBytes)
        {
            //arrange
            var sut = new BlowfishEncryptor(BlowfishCipherMode.Ecb);
            var expected = cipherBytes;
            //act
            var actual = sut.Encrypt(clearBytes, keyBytes);
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }
        public void Encrypt_KeySizeIs0_CanEncrypt()
        {
            throw new System.NotImplementedException();
        }
        
    }
}