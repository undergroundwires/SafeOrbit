using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Tests;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Cryptography.Encryption
{
    /// <seealso cref="ISafeEncryptor" />
    /// <seealso cref="AesEncryptor" />
    public class AesEncryptorTests : TestsFor<ISafeEncryptor>
    {
        protected override ISafeEncryptor GetSut() => new AesEncryptor();

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ThreeDifferentByteArrays16Length))]
        public async Task Decrypt_WhenDecryptedWithEncryptionKey_returnsInput(byte[] input, byte[] key, byte[] salt)
        {
            //Arrange
            var sut = GetSut();
            var expected = input;
            var encrypted = await sut.EncryptAsync(input, key, salt);
            //Act
            var actual = await sut.DecryptAsync(encrypted, key, salt);
            //Assert
            Assert.IsTrue(expected.SequenceEqual(actual));
        }
    }
}