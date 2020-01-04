using NUnit.Framework;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Memory.SafeBytesServices;
using SafeOrbit.Tests.Cases;

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
        [TestCase(1), TestCase(7), TestCase(8), TestCase(9), TestCase(15), TestCase(16), TestCase(17), TestCase(300)]
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