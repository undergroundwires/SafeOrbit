using System.Linq;
using NUnit.Framework;
using SafeOrbit.Cryptography.Encryption;
using Moq;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Fakes;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.SafeBytesServices.DataProtection.Protector
{
    [TestFixture]
    public class MemoryProtectorTests : TestsFor<IByteArrayProtector>
    {
        protected override IByteArrayProtector GetSut() => new MemoryProtector();

#if !NET45
        [Test]
        public void BlockSizeInBytes_GivenEncryptor_ReturnsFromEncryptor()
        {
            // Arrange
            const int encryptorBlockSizeInBits = 32;
            const int expected = 4;
            var mock = new Mock<IFastEncryptor>();
            mock.SetupGet(e=>e.BlockSizeInBits)
                .Returns(encryptorBlockSizeInBits);
            var protector = new MemoryProtector(
                encryptor: mock.Object,
                random: Stubs.Get<IFastRandom>());
            // Act
            var actual = protector.BlockSizeInBytes;
            // Assert
            Assert.AreEqual(expected, actual);
        }
#endif

        [Test]
        public void Protect_WhenByteArrayIsProtected_ByteArrayIsNotMemoryReadable()
        {
            //Arrange
            var sut = GetSut();
            var notExpected = new byte[]
            {
                10, 20, 30, 40, 50, 60, 80, 90, 100,
                110, 120, 130, 140, 150, 160, 170
            };
            var actual = notExpected.ToArray();
            //Act
            sut.Protect(actual);
            //Assert
            Assert.That(actual, Is.Not.EqualTo(notExpected));
        }

        [Test]
        public void Unprotect_AfterByteArrayIsProtected_BringsBackTheByteArray()
        {
            //Arrange
            var sut = GetSut();
            var expected = new byte[]
            {
                10, 20, 30, 40, 50, 60, 80, 90, 100,
                110, 120, 130, 140, 150, 160, 170
            };
            var actual = expected.ToArray();
            //Act
            sut.Protect(actual);
            sut.Unprotect(actual);
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}