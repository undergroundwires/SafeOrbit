using System.Linq;
using NUnit.Framework;
using SafeOrbit.Cryptography.Encryption.Padding;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Tests;
using SafeOrbit.Tests.Cases;
using SafeOrbit.Vectors;

namespace SafeOrbit.Cryptography.Encryption
{
    /// <seealso cref="IFastEncryptor" />
    /// <seealso cref="BlowfishEncryptor" />
    [TestFixture]
    public class BlowfishEncryptorTests : TestsFor<IFastEncryptor>
    {
        protected override IFastEncryptor GetSut() => new BlowfishEncryptor();

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
        [TestCaseSource(typeof(BlowfishVectors), nameof(BlowfishVectors.Vectors))]
        public void Encrypt_Ecb_ReturnsVector(byte[] keyBytes, byte[] clearBytes, byte[] cipherBytes)
        {
            //arrange
            var sut = new BlowfishEncryptor(BlowfishCipherMode.Ecb, PaddingMode.None);
            var expected = cipherBytes;
            //act
            var actual = sut.Encrypt(clearBytes, keyBytes);
            //assert
            Assert.That(actual, Is.EqualTo(expected));
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
        [TestCaseSource(typeof(BlowfishEncryptorTests), nameof(DataSizeTestCases))]
        public void Encrypt_DifferentDataSizes_CanDecrypt(int dataSize, BlowfishCipherMode cipherMode) // a.k.a. padding works
        {
            // Arrange
            var expected = FastRandom.StaticInstance.GetBytes(dataSize);
            var sut = new BlowfishEncryptor(cipherMode, PaddingMode.PKCS7);
            var key = new byte[sut.MinKeySizeInBits];
            // Act
            var encrypted = sut.Encrypt(expected, key);
            var actual = sut.Decrypt(encrypted, key);
            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        public static object[] DataSizeTestCases()
        {
            return GenerateTestCasesForBothCbcAndEcb(1, 7, 8, 9, 10, 300, 500001);
            static object[] GenerateTestCasesForBothCbcAndEcb(params int[] sizes) =>
                sizes.Select((size) => new object[]
                        {new object[] {size, BlowfishCipherMode.Cbc}, new object[] {size, BlowfishCipherMode.Ecb}})
                    .SelectMany(s => s)
                    .ToArray();
        }
    }
}