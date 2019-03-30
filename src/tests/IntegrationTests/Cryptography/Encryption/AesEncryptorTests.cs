using System.Linq;
using NUnit.Framework;
using SafeOrbit.Tests;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Cryptography.Encryption
{
    /// <seealso cref="ISafeEncryptor" />
    /// <seealso cref="AesEncryptor" />
    public class AesEncryptorTests : TestsFor<ISafeEncryptor>
    {
        protected override ISafeEncryptor GetSut() =>  new AesEncryptor();

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