using Moq;
using NUnit.Framework;
using SafeOrbit.Cryptography.Hashers;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;

namespace SafeOrbit.Memory.SafeBytesServices.Id
{
    /// <seealso cref="IByteIdGenerator" />
    /// <seealso cref="HashedByteIdGenerator" />
    /// <seealso cref="ByteIdGeneratorFaker" />
    public class HashedByteIdGeneratorTests
    {
        [Test]
        public void Ctor_HasherProducesDifferentResults_setsSalt()
        {
            // Arrange
            var mockHasher = new Mock<IFastHasher>();
            var sequence = mockHasher.SetupSequence(m => m.ComputeFast(It.IsAny<byte[]>()));
            for (var i = 0; i < 256; i++)
                sequence.Returns(i);
            var salt = Stubs.Get<IMemoryProtectedBytes>();
            GetSut(mockHasher.Object, sessionSalt: salt);
            // Act
            var sessionSalt = salt.RevealDecryptedBytes().PlainBytes;
            // Assert
            Assert.That(sessionSalt, Is.Not.Null);
            Assert.That(sessionSalt, Is.Not.Empty);
            Assert.That(sessionSalt, Has.Length.EqualTo(salt.BlockSizeInBytes));
        }

        [Test]
        public void Ctor_HasherProducesSameResultsFirstThenDifferent_setsSalt()
        {
            // Arrange
            var mockHasher = new Mock<IFastHasher>();
            var sequence = mockHasher.SetupSequence(m => m.ComputeFast(It.IsAny<byte[]>()));
            for (var i = 0; i < 1024; i++) sequence.Returns(0); //returns same 1024 times
            for (var i = 0; i < 256; i++) sequence.Returns(i); //starts returning unique
            var salt = Stubs.Get<IMemoryProtectedBytes>();
            GetSut(mockHasher.Object, sessionSalt: salt);
            // Act
            var sessionSalt = salt.RevealDecryptedBytes().PlainBytes;
            // Assert
            Assert.That(sessionSalt, Is.Not.Null);
            Assert.That(sessionSalt, Is.Not.Empty);
            Assert.That(sessionSalt, Has.Length.EqualTo(salt.BlockSizeInBytes));
        }

        [Test]
        public void Generate_GenerateInvokesHashMethodOfHasher_returnsTrue()
        {
            //Arrange
            var mockHasher = new Mock<IFastHasher>();
            var sequence = mockHasher.SetupSequence(m => m.ComputeFast(It.IsAny<byte[]>()));
            for (var i = 0; i < 256; i++)
                sequence.Returns(i);
            var salt = Stubs.Get<IMemoryProtectedBytes>();
            var sut = GetSut(mockHasher.Object, sessionSalt: salt);
            mockHasher.Reset();
            //Act
            sut.Generate(It.IsAny<byte>());
            //Assert
            mockHasher.Verify(x =>
                x.ComputeFast(It.IsNotNull<byte[]>()), Times.Exactly(1));
        }

        private HashedByteIdGenerator GetSut(IFastHasher hasher = null, ISafeRandom random = null,
            IMemoryProtectedBytes sessionSalt = null)
        {
            if (hasher == null)
            {
                var mockHasher = new Mock<IFastHasher>();
                mockHasher.Setup(m => m.ComputeFast(It.IsAny<byte[]>()))
#if !(NETCOREAPP3_1 || NETCOREAPP3_0)
                    .Returns((byte[] b) => b[b.Length - 1]);
#else
                    .Returns((byte[] b) => b[^1]);
#endif
                hasher = mockHasher.Object;
            }

            return new HashedByteIdGenerator(
                hasher, random ?? Stubs.Get<ISafeRandom>(),
                sessionSalt ?? Stubs.Get<IMemoryProtectedBytes>());
        }
    }
}