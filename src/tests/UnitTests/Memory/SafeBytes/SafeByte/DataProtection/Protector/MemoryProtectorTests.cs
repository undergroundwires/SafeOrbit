using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SafeOrbit.Cryptography.Encryption;
using SafeOrbit.Cryptography.Encryption.Padding;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Exceptions;
using SafeOrbit.Fakes;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector
{
    [TestFixture]
    public class MemoryProtectorTests : TestsFor<IByteArrayProtector>
    {
#if NET45
        protected override IByteArrayProtector GetSut() => new MemoryProtector();
#else
        protected override IByteArrayProtector GetSut() => GetSut();
#endif

#if !NET45
        [Test]
        public void BlockSizeInBytes_GivenEncryptor_ReturnsFromEncryptor()
        {
            // Arrange
            const int encryptorBlockSizeInBits = 32;
            const int expected = 4;
            var mock = new Mock<IFastEncryptor>();
            mock.SetupGet(e => e.BlockSizeInBits)
                .Returns(encryptorBlockSizeInBits);
            var protector = new MemoryProtector(
                encryptor: mock.Object,
                random: Stubs.Get<IFastRandom>());
            // Act
            var actual = protector.BlockSizeInBytes;
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Ctor_GivenEncryptor_DisablesPadding()
        {
            // Arrange
            const PaddingMode expected = PaddingMode.None;
            var mock = new Mock<IFastEncryptor>();
            mock.SetupProperty(s => s.Padding);
            // Act
            GetSut(encryptor: mock.Object);
            // Assert
            var actual = mock.Object.Padding;
            Assert.AreEqual(expected, actual);
        }
#endif

        [Test]
        public void ProtectAsync_InvalidBlockSize_Throws()
        {
            // Arrange
            var sut = GetSut();
            var invalidBytes = new byte[sut.BlockSizeInBytes - 1];

            // Act
            Task Action() => sut.ProtectAsync(invalidBytes);

            // Assert
            Assert.ThrowsAsync<DataLengthException>(Action);
        }

        [Test]
        public void UnprotectAsync_InvalidBlockSize_Throws()
        {
            // Arrange
            var sut = GetSut();
            var invalidBytes = new byte[sut.BlockSizeInBytes - 1];

            // Act
            Task Action() => sut.UnprotectAsync(invalidBytes);

            // Assert
            Assert.ThrowsAsync<DataLengthException>(Action);
        }

        [Test]
        public async Task ProtectAsync_WhenByteArrayIsProtected_ByteArrayIsNotMemoryReadable()
        {
            // Arrange
            var sut = GetSut();
            var notExpected = new byte[]
            {
                10, 20, 30, 40, 50, 60, 80, 90, 100,
                110, 120, 130, 140, 150, 160, 170
            };
            var actual = notExpected.ToArray();

            // Act
            await sut.ProtectAsync(actual).ConfigureAwait(false);

            // Assert
            Assert.That(actual, Is.Not.EqualTo(notExpected));
        }

        [Test]
        public async Task UnprotectAsync_AfterByteArrayIsProtected_BringsBackTheByteArray()
        {
            // Arrange
            var sut = GetSut();
            var expected = new byte[]
            {
                10, 20, 30, 40, 50, 60, 80, 90, 100,
                110, 120, 130, 140, 150, 160, 170
            };
            var actual = expected.ToArray();

            // Act
            await sut.ProtectAsync(actual);
            await sut.UnprotectAsync(actual);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }


#if !NET45
        private static IByteArrayProtector GetSut(IFastEncryptor encryptor = null, IFastRandom random = null) =>
            new MemoryProtector(
                encryptor: encryptor ?? Stubs.Get<IFastEncryptor>(), random: random ?? Stubs.Get<IFastRandom>());
#endif
    }
}