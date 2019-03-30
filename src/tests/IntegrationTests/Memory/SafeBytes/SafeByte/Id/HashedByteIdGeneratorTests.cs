using System.Linq;
using NUnit.Framework;
using SafeOrbit.Tests;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Memory.SafeBytesServices.Id
{
    /// <seealso cref="HashedByteIdGenerator" />
    /// <seealso cref="IByteIdGenerator" />
    internal class HashedByteIdGeneratorTests : TestsFor<HashedByteIdGenerator>
    {
        [Test]
        public void Generate_ResultForAllPossibleBytes_areUnique()
        {
            //Arrange
            var sut = GetSut();
            var hashedValues = new int[256];
            for (var i = 0; i < 256; i++)
                hashedValues[i] = sut.Generate((byte) i);
            //Act
            const int expected = 256;
            var actual = hashedValues.Distinct().Count();
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Generate_SameInstanceForSameBytes_givesSameResults([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var expected = sut.Generate(b);
            var actual = sut.Generate(b);
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentBytePairs))]
        public void Generate_ForDifferentBytes_areNotEqual(byte b1, byte b2)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var actual = sut.Generate(b1);
            var expected = sut.Generate(b2);
            //Assert
            Assert.AreNotEqual(actual, expected);
        }
        protected override HashedByteIdGenerator GetSut() => new HashedByteIdGenerator();
    }
}