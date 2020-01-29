using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Extensions;
using SafeOrbit.Memory.SafeBytesServices;

namespace SafeOrbit.Memory
{
    /// <seealso cref="ISafeBytes"/>
    /// <seealso cref="SafeBytes"/>
    /// <seealso cref="SafeByteTests"/>
    [TestFixture]
    public class SafeBytesTests
    {
        [Test]
        public async Task RevealDecryptedByteAsync_SingleByteAppended_ReturnsExpected()
        {
            // Arrange
            const byte expected = 31;
            using var sut = GetSut();
            
            // Act
            await sut.AppendAsync(expected);
            var actual = await sut.RevealDecryptedByteAsync(0);
            
            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task AppendManyAsync_SomeBytesAdded_CanRetrieve()
        {
            // Arrange
            using var sut = GetSut();
            var expected = new byte[] {5, 10, 15};
            using var stream = new SafeMemoryStream(expected.CopyToNewArray());
            
            // Act
            await sut.AppendManyAsync(stream);
            var actual = await sut.RevealDecryptedBytesAsync();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(1), TestCase(7), TestCase(8), TestCase(9), TestCase(15), TestCase(16), TestCase(17)]  // Boundary values for block sizes
        public async Task ToByteArrayAsync_BytesAppended_ReturnsExpected(int size)
        {
            // Arrange
            var expected = FastRandom.StaticInstance.GetBytes(size);
            var sut = GetSut();

            // Act
            foreach (var @byte in expected)
                await sut.AppendAsync(@byte);
            var actual = await sut.RevealDecryptedBytesAsync();

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }


        [Test]
        public async Task EqualsAsync_SafeBytesHoldingSameBytes_ReturnsTrue()
        {
            // Arrange
            using var sut = GetSut();
            var bytes = new byte[] { 5, 10, 15 };
            var stream = new SafeMemoryStream(bytes.CopyToNewArray());
            await sut.AppendManyAsync(stream);

            using var other = GetSut();
            stream = new SafeMemoryStream(bytes.CopyToNewArray());
            await other.AppendManyAsync(stream);

            // Act
            var actual = await sut.EqualsAsync(other);

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public async Task EqualsAsync_SafeBytesDifferentSameBytes_ReturnsFalse()
        {
            // Arrange
            using var sut = GetSut();
            var stream = new SafeMemoryStream(new byte[] { 1, 2, 3 });
            await sut.AppendManyAsync(stream);

            using var other = GetSut();
            stream = new SafeMemoryStream(new byte[] { 4, 5 });
            await other.AppendManyAsync(stream);

            // Act
            var actual = await sut.EqualsAsync(other);

            // Assert
            Assert.IsFalse(actual);
        }


        private static ISafeBytes GetSut() => new SafeBytes();
    }
}