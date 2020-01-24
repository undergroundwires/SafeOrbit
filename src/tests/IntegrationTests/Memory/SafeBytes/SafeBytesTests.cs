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
        public void GetByte_SingleByteAppended_ReturnsExpected([Random(0, 256, 1)] byte b)
        {
            // Arrange
            var expected = b;
            using var sut = GetSut();
            // Act
            sut.Append(expected);
            var actual = sut.GetByte(0);
            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void AppendMany_SomeBytesAdded_CanRetrieve()
        {
            // Arrange
            using var sut = GetSut();
            var expected = new byte[] {5, 10, 15};
            using var stream = new SafeMemoryStream();
            stream.Write(expected.CopyToNewArray(), 0, expected.Length);
            
            // Act
            sut.AppendMany(stream);
            var actual = sut.ToByteArray();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(1), TestCase(7), TestCase(8), TestCase(9), TestCase(15), TestCase(16), TestCase(17)]
        public void ToByteArray_BytesAppended_ReturnsExpected(int size)
        {
            // Arrange
            var expected = FastRandom.StaticInstance.GetBytes(size);
            var sut = GetSut();
            // Act
            foreach (var @byte in expected)
                sut.Append(@byte);
            var actual = sut.ToByteArray();
            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        private static ISafeBytes GetSut() => new SafeBytes();
    }
}