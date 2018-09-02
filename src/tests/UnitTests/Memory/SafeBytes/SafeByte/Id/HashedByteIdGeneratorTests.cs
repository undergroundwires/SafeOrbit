using System.Linq;
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
    /// <seealso cref="ByteIdGeneratorFaker"/>
    public class HashedByteIdGeneratorTests
    {
        [Test]
        public void SessionSalt_ForDifferentInstances_IsSame()
        {
            //arrange
            var expected = GetSut().SessionSalt;
            var instances = new HashedByteIdGenerator[20];
            for (var i = 0; i < instances.Length; i++)
                instances[i] = GetSut();
            //act
            var results = instances.Select(i => i.SessionSalt);
            //assert
            Assert.That(results, Is.All.SameAs(expected));
        }

        [Test]
        public void SessionSalt_HasherProducesDifferentResults_IsNotNullOrEmpty()
        {
            //Arrange
            var mockHasher = new Mock<IFastHasher>();
            var sequence = mockHasher.SetupSequence(m => m.ComputeFast(It.IsAny<byte[]>()));
            for (var i = 0; i < 256; i++) sequence.Returns(i); //returns different 256 times
            var sut = GetSut(mockHasher.Object);
            //Act
            var sessionSalt = sut.SessionSalt;
            //Assert
            Assert.That(sessionSalt, Is.Not.Null);
            Assert.That(sessionSalt, Is.Not.Empty);
        }

        [Test]
        public void SessionSalt_HasherProducesSameResultsFirstThenDifferent_IsNotNullOrEmpty()
        {
            //Arrange
            var mockHasher = new Mock<IFastHasher>();
            var sequence = mockHasher.SetupSequence(m => m.ComputeFast(It.IsAny<byte[]>()));
            for (var i = 0; i < 1024; i++) sequence.Returns(0); //returns same 1024 times
            for (var i = 0; i < 256; i++) sequence.Returns(i); //starts returning unique
            var sut = GetSut(mockHasher.Object);
            //Act
            var sessionSalt = sut.SessionSalt;
            //Assert
            Assert.That(sessionSalt, Is.Not.Null);
            Assert.That(sessionSalt, Is.Not.Empty);
        }


        [Test]
        public void SessionSalt_RealBytesNotVisible()
        {
            //arrange
            var randomData = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            var protectedData = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            var expected = protectedData;
            var randomMock = new Mock<ISafeRandom>();
            randomMock.Setup(m => m.GetBytes(16)).Returns(randomData);
            var protectorMock = new Mock<IByteArrayProtector>();
            protectorMock.Setup(m => m.Protect(It.Is<byte[]>(bytes => bytes.SequenceEqual(randomData))))
                .Callback((byte[] array) =>
                {
                    for (var i = 0; i < array.Length; i++) array[i] = protectedData[i];
                });
            protectorMock.Setup(m => m.Unprotect(It.Is<byte[]>(bytes => bytes.SequenceEqual(protectedData))))
                .Callback((byte[] array) =>
                {
                    for (var i = 0; i < array.Length; i++) array[i] = randomData[i];
                });
            var sut = GetSut(random: randomMock.Object, protector: protectorMock.Object);
            //act
            var actual = sut.SessionSalt.ToArray();
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }


        [Test]
        public void Generate_GenerateInvokesHashMethodOfHasher_returnsTrue()
        {
            //Arrange
            var mockHasher = new Mock<IFastHasher>();
            var sequence = mockHasher.SetupSequence(m => m.ComputeFast(It.IsAny<byte[]>()));
            for (var i = 0; i < 256; i++) sequence.Returns(i); //returns different 256 times
            var sut = GetSut(mockHasher.Object);
            var temp = sut.SessionSalt; //reaching getter will produce the session salt
            mockHasher.Reset();
            //Act
            sut.Generate(It.IsAny<byte>());
            //Assert
            mockHasher.Verify(x =>
                    x.ComputeFast(It.IsNotNull<byte[]>()), Times.Exactly(1));
        }

        private HashedByteIdGenerator GetSut(IFastHasher hasher = null, ISafeRandom random = null, IByteArrayProtector protector = null)
        {
            if (hasher == null)
            {
                var mockHasher = new Mock<IFastHasher>();
                mockHasher.Setup(m => m.ComputeFast(It.IsAny<byte[]>())).Returns((byte[] b) => b[b.Length - 1]);
                hasher = mockHasher.Object;
            }
            return new HashedByteIdGenerator(hasher, random ?? Stubs.Get<ISafeRandom>(), protector ?? Mock.Of<IByteArrayProtector>());
        }
    }
}