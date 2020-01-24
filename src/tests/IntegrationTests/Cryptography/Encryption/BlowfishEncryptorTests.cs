using System.Linq;
using System.Threading.Tasks;
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
        public async Task Decrypt_Cbc_WhenDecryptedWithEncryptionKey_returnsInput(byte[] input, byte[] key)
        {
            // Arrange
            var sut = new BlowfishEncryptor(BlowfishCipherMode.Cbc);
            var encrypted = await sut.EncryptAsync(input, key);

            // Act
            var output = await sut.DecryptAsync(encrypted, key);

            // Assert
            Assert.IsTrue(input.SequenceEqual(output));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentByteArrayPairs32Length))]
        public async Task Decrypt_Ecb_WhenDecryptedWithEncryptionKey_returnsInput(byte[] input, byte[] key)
        {
            // Arrange
            var sut = new BlowfishEncryptor(BlowfishCipherMode.Ecb);
            var encrypted = await sut.EncryptAsync(input, key);

            // Act
            var output = await sut.DecryptAsync(encrypted, key);

            // Assert
            Assert.IsTrue(input.SequenceEqual(output));
        }

        [Test]
        [TestCaseSource(typeof(BlowfishVectors), nameof(BlowfishVectors.Vectors))]
        public async Task Encrypt_Ecb_ReturnsVector(byte[] keyBytes, byte[] clearBytes, byte[] cipherBytes)
        {
            // Arrange
            var sut = new BlowfishEncryptor(BlowfishCipherMode.Ecb, PaddingMode.None);
            var expected = cipherBytes;

            // Act
            var actual = await sut.EncryptAsync(clearBytes, keyBytes);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentByteArrayPairs32Length))]
        public async Task Encrypt_returnsDifferentValueThanInput(byte[] input, byte[] key)
        {
            // Arrange
            var sut = GetSut();

            // Act
            var output = await sut.EncryptAsync(input, key);

            // Assert
            Assert.IsFalse(input.SequenceEqual(output));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ThreeDifferentByteArrays16Length))]
        public async Task Encrypt_WhenDifferentKeysGiven_returnsDifferentValues(byte[] input, byte[] key, byte[] differentKey)
        {
            // Arrange
            var sut = GetSut();
            
            // Act
            var output = await sut.EncryptAsync(input, key);
            var outputFromDifferentKey = await sut.EncryptAsync(input, differentKey);
            
            // Assert
            Assert.IsFalse(output.SequenceEqual(outputFromDifferentKey));
        }

        [Test]
        [TestCaseSource(typeof(BlowfishEncryptorTests), nameof(DataSizeTestCases))]
        public async Task Encrypt_DifferentDataSizes_CanDecrypt(int dataSize,
            BlowfishCipherMode cipherMode) // a.k.a. padding works
        {
            // Arrange
            var expected = FastRandom.StaticInstance.GetBytes(dataSize);
            var sut = new BlowfishEncryptor(cipherMode, PaddingMode.PKCS7);
            var key = new byte[sut.MinKeySizeInBits];

            // Act
            var encrypted = await sut.EncryptAsync(expected, key);
            var actual = await sut.DecryptAsync(encrypted, key);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        public static object[] DataSizeTestCases()
        {
            return GenerateTestCasesForBothCbcAndEcb(1, 7, 8, 9, 10, 300, 500001);

            static object[] GenerateTestCasesForBothCbcAndEcb(params int[] sizes) =>
                sizes.Select(size => new object[]
                        {new object[] {size, BlowfishCipherMode.Cbc}, new object[] {size, BlowfishCipherMode.Ecb}})
                    .SelectMany(s => s)
                    .ToArray();
        }
    }
}