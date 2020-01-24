using System;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task Salt_HasherProducesDifferentResults_IsSet()
        {
            // Arrange
            var mockHasher = new Mock<IFastHasher>();
            var sequence = mockHasher.SetupSequence(m => m.ComputeFast(It.IsAny<byte[]>()));
            for (var i = 0; i < 256; i++)
                sequence.Returns(i);
            var salt = Stubs.Get<IMemoryProtectedBytes>();
            var sut = GetSut(mockHasher.Object, sessionSalt: salt);

            // Act
            _ = await sut.GenerateAsync(5); // to trigger lazy initialization
            var sessionSalt = (await salt.RevealDecryptedBytesAsync()).PlainBytes;

            // Assert
            Assert.That(sessionSalt, Is.Not.Null);
            Assert.That(sessionSalt, Is.Not.Empty);
            Assert.That(sessionSalt, Has.Length.EqualTo(salt.BlockSizeInBytes));
        }

        [Test]
        public async Task Salt_HasherProducesSameResultsFirstThenDifferent_IsSet()
        {
            // Arrange
            var mockHasher = new Mock<IFastHasher>();
            var sequence = mockHasher.SetupSequence(m => m.ComputeFast(It.IsAny<byte[]>()));
            for (var i = 0; i < 1024; i++) sequence.Returns(0); //returns same 1024 times
            for (var i = 0; i < 256; i++) sequence.Returns(i); //starts returning unique
            var salt = Stubs.Get<IMemoryProtectedBytes>();
            var sut = GetSut(mockHasher.Object, sessionSalt: salt);
            var dummyStream = new SafeMemoryStream();
            dummyStream.Write(new byte[5], 0,5);

            // Act
            _ = await sut.GenerateManyAsync(dummyStream); // Will trigger lazy initialization
            var sessionSalt = (await salt.RevealDecryptedBytesAsync()).PlainBytes;

            // Assert
            Assert.That(sessionSalt, Is.Not.Null);
            Assert.That(sessionSalt, Is.Not.Empty);
            Assert.That(sessionSalt, Has.Length.EqualTo(salt.BlockSizeInBytes));
        }

        [Test]
        public async Task GenerateAsync_GenerateInvokesHashMethodOfHasher_returnsTrue()
        {
            // Arrange
            var mockHasher = new Mock<IFastHasher>();
            var sequence = mockHasher.SetupSequence(m => m.ComputeFast(It.IsAny<byte[]>())); 
            for (var i = 0; i < 256; i++) 
                sequence.Returns(i);
            var salt = Stubs.Get<IMemoryProtectedBytes>();
            var sut = GetSut(mockHasher.Object, sessionSalt: salt);
            await sut.GenerateAsync(It.IsAny<byte>()); // To ensure salt is initialized
            mockHasher.Reset();

            // Act
            await sut.GenerateAsync(It.IsAny<byte>());

            // Assert
            mockHasher.Verify(x =>
                x.ComputeFast(It.IsNotNull<byte[]>()), Times.Exactly(1));
        }

        [Test]
        public async Task GenerateManyAsync_EmptyStream_ReturnsEmpty()
        {
            // Arrange
            var sut = GetSut();

            // Act
            var actual = await sut.GenerateManyAsync(new SafeMemoryStream());

            // Assert
            Assert.IsEmpty(actual);
        }

        [Test]
        public void GenerateManyAsync_NullStream_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            SafeMemoryStream stream = null;

            // Act
            async Task Act() => _ = (await sut.GenerateManyAsync(stream)).ToArray();

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(Act);
        }

        [Test]
        public async Task GenerateManyAsync_GivenStream_ReturnsExpectedLength()
        {
            //Arrange
            var sut = GetSut();
            const int expected = 55;
            var input = new byte[expected];
            var stream = new SafeMemoryStream();
            stream.Write(input, 0, input.Length);

            //Act
            var actual = (await sut.GenerateManyAsync(stream)).Count();

            //Assert
            Assert.AreEqual(expected, actual);
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