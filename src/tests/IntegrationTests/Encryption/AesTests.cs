
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Encryption;
using SafeOrbit.Tests.Cases;
using SafeOrbit.Tests;

namespace SafeOrbit.IntegrationTests.Encryption
{
    public class AesTests : TestsFor<ISafeEncryptor>
    {
        protected override ISafeEncryptor GetSut()
        {
            return new AesEncryptor();
        }
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ThreeDifferentByteArrays16Length))]
        public void Decrypt_WhenDecryptedWithEncryptionKey_returnsInput(byte[] input, byte[] key, byte[] salt)
        {
            //Arrange
            var sut = GetSut();
            var expected = input;
            var encrypted = sut.Encrypt(input, key, salt);
            //Act
            var actual = sut.Decrypt(encrypted, key, salt);
            //Assert
            Assert.IsTrue(expected.SequenceEqual(actual));
        }
    }
}
