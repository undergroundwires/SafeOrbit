using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Extensions;
using SafeOrbit.Tests;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Memory.SafeBytesServices.Id
{
    /// <seealso cref="HashedByteIdGenerator" />
    /// <seealso cref="IByteIdGenerator" />
    internal class HashedByteIdGeneratorTests : TestsFor<HashedByteIdGenerator>
    {
        [Test]
        public async Task GenerateAsync_ResultForAllPossibleBytes_AreUnique()
        {
            // Arrange
            var sut = GetSut();
            var hashedValues = new int[256];
            for (var i = 0; i < 256; i++)
                hashedValues[i] = await sut.GenerateAsync((byte) i);

            // Act
            const int expected = 256;
            var actual = hashedValues.Distinct().Count();

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task GenerateAsync_SameInstanceForSameBytes_GivesSameResults([Random(0, 256, 1)] byte b)
        {
            // Arrange
            var sut = GetSut();

            // Act
            var expected = await sut.GenerateAsync(b);
            var actual = await sut.GenerateAsync(b);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentBytePairs))]
        public async Task GenerateAsync_ForDifferentBytes_AreNotEqual(byte b1, byte b2)
        {
            // Arrange
            var sut = GetSut();

            // Act
            var actual = await sut.GenerateAsync(b1);
            var expected = await sut.GenerateAsync(b2);

            // Assert
            Assert.AreNotEqual(expected, actual);
        }

        [Test]
        public async Task GenerateManyAsync_SameInstanceForSameBytes_GivesSameResults()
        {
            // Arrange
            var bytes = new byte[] {1, 2, 3, 4, 5};
            var sut = GetSut();

            // Act
            var expected = await sut.GenerateManyAsync( new SafeMemoryStream(bytes.CopyToNewArray()));
            var actual = await sut.GenerateManyAsync(new SafeMemoryStream(bytes.CopyToNewArray()));

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public async Task GenerateManyAsync_ForDifferentBytes_AreNotEqual()
        {
            // Arrange
            var sut = GetSut();

            // Act
            var results = await sut.GenerateManyAsync( new SafeMemoryStream(new byte[] { 1,2,3,4,5 }));
            var differentResults = await sut.GenerateManyAsync(new SafeMemoryStream(new byte[] { 6,7,8,9,10 }));

            // Assert
            Assert.True(results.All(@byte => !differentResults.Contains(@byte)));
        }

        protected override HashedByteIdGenerator GetSut() => new HashedByteIdGenerator();
    }
}