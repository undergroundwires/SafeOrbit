using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SafeOrbit.Memory
{
    /// <seealso cref="SafeString" />
    /// <seealso cref="ISafeString" />
    [TestFixture]
    public class SafeStringTests
    {
        [Test]
        public async Task SafeString_MultipleCharsAdded_MarshalsToExpected()
        {
            // Arrange
            const string expected = "test";
            var sut = new SafeString();

            // Act
            await sut.AppendAsync("t");
            await sut.AppendAsync("es");
            await sut.AppendAsync('t');

            // Assert
            using var sm = new SafeStringToStringMarshaler(sut);
            var actual = sm.String;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task SafeString_ModifiedByAnotherThread_ModifiesAsExpected()
        {
            // Arrange
            var sut = new SafeString();
            await sut.AppendAsync("test");

            // Act
            var thread = new Thread(() =>
            {
                sut.Remove(1);
                sut.Remove(1);
            });
            thread.Start();
            thread.Join();

            // Assert
            using var sm = new SafeStringToStringMarshaler(sut);
            var actual = sm.String;
            Assert.That(actual, Is.EqualTo("tt"));
        }

        [Test]
        public async Task EqualsAsync_TwoStringsWithDifferentLength_ReturnsFalse()
        {
            // Arrange
            var sut1 = new SafeString();
            await sut1.AppendAsync("hej");
            var sut2 = new SafeString();
            await sut2.AppendAsync("sp");

            // Act
            var equality = await sut1.EqualsAsync(sut2);
            var equalityOpposite = await sut2.EqualsAsync(sut1);

            // Assert
            Assert.False(equality);
            Assert.False(equalityOpposite);
        }

        [Test]
        public async Task SafeString_TextCanBeRevealedTwice()
        {
            // Arrange
            const string expected = "test";
            var sut = new SafeString();
            await sut.AppendAsync(expected);

            // Act
            var revealed = await RevealAsync(sut);
            var revealedAgain = await RevealAsync(sut);

            // Assert
            Assert.AreEqual(expected, revealed);
            Assert.AreEqual(expected, revealedAgain);

            static async Task<string> RevealAsync(ISafeString str)
            {
                var strBuilder = new StringBuilder(str.Length);
                for (var i = 0; i < str.Length; i++)
                    strBuilder.Append(await str.RevealDecryptedCharAsync(i));
                return strBuilder.ToString();
            }
        }

        [Test]
        public async Task RevalDecryptedBytesAsync_AppendedUnicodeString_ReturnsExpected()
        {
            // Arrange
            const string str = "hello";
            var expected = new byte[] {104, 0, 101, 0, 108, 0, 108, 0, 111, 0};
            var sut = new SafeString();
            await sut.AppendAsync(str);

            // Act
            var actual = await sut.RevealDecryptedBytesAsync();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}