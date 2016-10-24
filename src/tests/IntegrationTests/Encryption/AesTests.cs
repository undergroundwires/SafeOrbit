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
