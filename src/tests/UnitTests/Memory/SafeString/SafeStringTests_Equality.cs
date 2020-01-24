using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SafeOrbit.Memory
{
    /// <seealso cref="ISafeString" />
    /// <seealso cref="SafeString" />
    [TestFixture]
    public partial class SafeStringTests
    {
        [Test]
        public void EqualsAsyncISafeString_NullArgument_Throws()
        {
            // Arrange
            using var sut = GetSut();
            ISafeString nullInstance = null;

            // Act
            Task Equals() => sut.EqualsAsync(nullInstance);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(Equals);
        }

        [Test]
        public async Task EqualsAsyncISafeString_ForDifferentInstancesHoldingSameChars_ReturnsTrue([Random(0, 256, 1)] byte i1,
            [Random(0, 256, 1)] byte i2)
        {
            // Arrange
            var sut = GetSut();
            var holdingSame = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2;
            await sut.AppendAsync(ch1);
            await sut.AppendAsync(ch2);
            await holdingSame.AppendAsync(ch1);
            await holdingSame.AppendAsync(ch2);

            // Act
            var equals = await sut.EqualsAsync(holdingSame);
            var equalsOpposite = await holdingSame.EqualsAsync(sut);

            // Assert
            Assert.That(equals, Is.True);
            Assert.That(equalsOpposite, Is.True);
        }

        [Test]
        public async Task EqualsAsyncISafeString_ForEmptyInstances_ReturnsTrue()
        {
            // Arrange
            var sut = GetSut();
            var empty = GetSut();

            // Act
            var equals = await sut.EqualsAsync(empty);
            var equalsOpposite = await empty.EqualsAsync(sut);
            
            // Assert
            Assert.That(equals, Is.True);
            Assert.That(equalsOpposite, Is.True);
        }

        [Test]
        public async Task EqualsAsyncISafeString_ForInstancesHoldingDifferentChars_ReturnsFalse([Random(0, 125, 1)] byte i1,
            [Random(125, 256, 1)] byte i2)
        {
            // Arrange
            var sut = GetSut();
            var holdingDifferent = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2;
            await sut.AppendAsync(ch1);
            await holdingDifferent.AppendAsync(ch2);

            // Act
            var equals = await sut.EqualsAsync(holdingDifferent);
            var equalsOpposite = await holdingDifferent.EqualsAsync(sut);
            
            // Assert
            Assert.That(equals, Is.False);
            Assert.That(equalsOpposite, Is.False);
        }

        [Test]
        public async Task EqualsAsyncISafeString_ForInstancesHoldingSameCharsInDifferentOrder_ReturnsFalse(
            [Random(0, 125, 1)] byte i1, [Random(125, 256, 1)] byte i2)
        {
            // Arrange
            var sut = GetSut();
            var holdingDifferent = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2;
            await sut.AppendAsync(ch1);
            await sut.AppendAsync(ch2);
            await holdingDifferent.AppendAsync(ch2);
            await holdingDifferent.AppendAsync(ch1);
            
            // Act
            var equals = await sut.EqualsAsync(holdingDifferent);
            var equalsOpposite = await holdingDifferent.EqualsAsync(sut);

            // Assert
            Assert.That(equals, Is.False);
            Assert.That(equalsOpposite, Is.False);
        }

        [Test]
        public async Task EqualsAsyncISafeString_ForSameObjects_ReturnsTrue([Random(0, 256, 1)] byte i)
        {
            // Arrange
            var sut = GetSut();
            var ch = (char) i;
            await sut.AppendAsync(ch);
            var sameObject = sut;

            // Act
            var equals = await sut.EqualsAsync(sameObject);
            var equalsOpposite = await sameObject.EqualsAsync(sut);

            // Assert
            Assert.That(equals, Is.True);
            Assert.That(equalsOpposite, Is.True);
        }

        [Test]
        public async Task EqualsAsyncISafeString_DifferentInstancesWithSameString_ReturnsTrue()
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync("Hello");
            var other = GetSut();
            await other.AppendAsync("Hello");

            // Act
            var equals = await sut.EqualsAsync(other);
            var equalsOpposite = await other.EqualsAsync(sut);

            // Assert
            Assert.That(equals, Is.True);
            Assert.That(equalsOpposite, Is.True);
        }

        [TestCase("Hello", "H3ll0")]
        [TestCase("Hello", "Hello2")]
        [TestCase("Hello2", "Hello")]
        public async Task EqualsAsyncISafeString_DifferentStrings_ReturnsFalse(string string1, string string2)
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync(string1);
            var other = GetSut();
            await other.AppendAsync(string2);

            // Act
            var equals = await sut.EqualsAsync(other);
            var equalsOpposite = await other.EqualsAsync(sut);
            
            // Assert
            Assert.That(equals, Is.False);
            Assert.That(equalsOpposite, Is.False);
        }


        [Test]
        public async Task EqualsAsyncString_WhenHoldingSameChars_ReturnsTrue()
        {
            // Arrange
            const char ch1 = 'a', ch2 = 'b';
            const string expected = "ab";
            using var sut = GetSut();
            await sut.AppendAsync(ch1);
            await sut.AppendAsync(ch2);

            // Act
            var equals = await sut.EqualsAsync(expected);

            // Assert
            Assert.True(equals);
        }

        [Test]
        public void EqualsAsyncString_NullArgument_Throws()
        {
            // Arrange
            using var sut = GetSut();
            string nullString = null;

            // Act
            Task Equals() => sut.EqualsAsync(nullString);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(Equals);
        }

        [Test]
        public async Task EqualsAsyncString_WhenHoldingDifferentChars_ReturnsFalse([Random(0, 125, 1)] byte i1,
            [Random(125, 256, 1)] byte i2)
        {
            // Arrange
            using var sut = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2;
            await sut.AppendAsync(ch1);
            await sut.AppendAsync(ch2);
            var text = $"{ch2}{ch1}";

            // Act
            var equals = await sut.EqualsAsync(text);

            // Assert
            Assert.That(equals, Is.False);
        }

        [Test]
        public void GetHashCode_ForDistinctEmptyInstances_ReturnsTrue()
        {
            // Arrange
            using var sut = GetSut();
            var holdingSame = GetSut();
            var hashCode = sut.GetHashCode();
            var sameHashCode = holdingSame.GetHashCode();

            // Assert
            Assert.That(hashCode, Is.EqualTo(sameHashCode));
        }

        [Test]
        public async Task GetHashCode_ForDistinctObjectsHavingSameMultipleChars_ReturnsTrue(
            [Random(0, 256, 1)] int i1, [Random(0, 256, 1)] int i2, [Random(0, 256, 1)] int i3)
        {
            // Arrange
            using var sut = GetSut();
            using var holdingSame = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2, ch3 = (char) i3;
            await sut.AppendAsync(ch1);
            await sut.AppendAsync(ch2);
            await sut.AppendAsync(ch3);
            await holdingSame.AppendAsync(ch1);
            await holdingSame.AppendAsync(ch2);
            await holdingSame.AppendAsync(ch3);

            // Act
            var hashCode = sut.GetHashCode();
            var sameHashCode = holdingSame.GetHashCode();

            // Assert
            Assert.That(hashCode, Is.EqualTo(sameHashCode));
        }

        //** GetHashCode() **//
        [Test]
        public async Task GetHashCode_ForDistinctObjectsHavingSameSingleChar_ReturnsTrue([Random(0, 256, 1)] int i)
        {
            // Arrange
            using var sut = GetSut();
            using var holdingSame = GetSut();
            var ch = (char) i;
            await sut.AppendAsync(ch);
            await holdingSame.AppendAsync(ch);

            // Act
            var hashCode = sut.GetHashCode();
            var sameHashCode = holdingSame.GetHashCode();

            // Assert
            Assert.That(hashCode, Is.EqualTo(sameHashCode));
        }

        [Test]
        public async Task GetHashCode_ForTwoNonEqualObjects_ReturnsFalse(
            [Random(0, 256, 1)] int i1, [Random(0, 256, 1)] int i2, [Random(0, 256, 1)] int i3)
        {
            // Arrange
            using var sut = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2, ch3 = (char) i3;
            await sut.AppendAsync(ch1);
            await sut.AppendAsync(ch2);
            var holdingDifferent = GetSut();
            await holdingDifferent.AppendAsync(ch3);
           
            // Act
            var hashCode = sut.GetHashCode();
            var hashCodeForDifferentObject = holdingDifferent.GetHashCode();
           
            // Assert
            Assert.That(hashCode, Is.Not.EqualTo(hashCodeForDifferentObject));
        }
    }
}