using NUnit.Framework;

namespace SafeOrbit.Memory
{
    /// <seealso cref="ISafeString" />
    /// <seealso cref="SafeString" />
    [TestFixture]
    public partial class SafeStringTests
    {
        [Test]
        public void Equals_ParameterIsNullObject_returnsFalse()
        {
            //Arrange
            using var sut = GetSut();
            object nullInstance = null;
            //Act
            var equals = sut.Equals(nullInstance);
            //Assert
            Assert.That(equals, Is.False);
        }

        [Test]
        public void EqualsISafeString_ForDifferentInstancesHoldingSameChars_returnsTrue([Random(0, 256, 1)] byte i1,
            [Random(0, 256, 1)] byte i2)
        {
            //Arrange
            var sut = GetSut();
            var holdingSame = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2;
            sut.Append(ch1);
            sut.Append(ch2);
            holdingSame.Append(ch1);
            holdingSame.Append(ch2);
            //Act
            var equals = sut.Equals(holdingSame);
            var equalsOpposite = holdingSame.Equals(sut);
            //Assert
            Assert.That(equals, Is.True);
            Assert.That(equalsOpposite, Is.True);
        }

        [Test]
        public void EqualsISafeString_ForEmptyInstances_returnsTrue()
        {
            //Arrange
            var sut = GetSut();
            var empty = GetSut();
            //Act
            var equals = sut.Equals(empty);
            var equalsOpposite = empty.Equals(sut);
            //Assert
            Assert.That(equals, Is.True);
            Assert.That(equalsOpposite, Is.True);
        }

        [Test]
        public void EqualsISafeString_ForInstancesHoldingDifferentChars_returnsFalse([Random(0, 125, 1)] byte i1,
            [Random(125, 256, 1)] byte i2)
        {
            //Arrange
            var sut = GetSut();
            var holdingDifferent = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2;
            sut.Append(ch1);
            holdingDifferent.Append(ch2);
            //Act
            var equals = sut.Equals(holdingDifferent);
            var equalsOpposite = holdingDifferent.Equals(sut);
            //Assert
            Assert.That(equals, Is.False);
            Assert.That(equalsOpposite, Is.False);
        }

        [Test]
        public void EqualsISafeString_ForInstancesHoldingSameCharsInDifferentOrder_returnsFalse(
            [Random(0, 125, 1)] byte i1, [Random(125, 256, 1)] byte i2)
        {
            //Arrange
            var sut = GetSut();
            var holdingDifferent = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2;
            sut.Append(ch1);
            sut.Append(ch2);
            holdingDifferent.Append(ch2);
            holdingDifferent.Append(ch1);
            //Act
            var equals = sut.Equals(holdingDifferent);
            var equalsOpposite = holdingDifferent.Equals(sut);
            //Assert
            Assert.That(equals, Is.False);
            Assert.That(equalsOpposite, Is.False);
        }

        [Test]
        public void EqualsISafeString_ForSameObjects_returnsTrue([Random(0, 256, 1)] byte i)
        {
            //Arrange
            var sut = GetSut();
            var ch = (char) i;
            sut.Append(ch);
            var sameObject = sut;
            //Act
            var equals = sut.Equals(sameObject);
            var equalsOpposite = sameObject.Equals(sut);
            //Assert
            Assert.That(equals, Is.True);
            Assert.That(equalsOpposite, Is.True);
        }

        [Test]
        public void EqualsISafeString_DifferentInstancesWithSameString_returnsTrue()
        {
            //Arrange
            var sut = GetSut();
            sut.Append("Hello");
            var other = GetSut();
            other.Append("Hello");
            //Act
            var equals = sut.Equals(other);
            var equalsOpposite = other.Equals(sut);
            //Assert
            Assert.That(equals, Is.True);
            Assert.That(equalsOpposite, Is.True);
        }

        [TestCase("Hello", "H3ll0")]
        [TestCase("Hello", "Hello2")]
        [TestCase("Hello2", "Hello")]
        public void EqualsISafeString_DifferentStrings_returnsFalse(string string1, string string2)
        {
            //Arrange
            var sut = GetSut();
            sut.Append(string1);
            var other = GetSut();
            other.Append(string2);
            //Act
            var equals = sut.Equals(other);
            var equalsOpposite = other.Equals(sut);
            //Assert
            Assert.That(equals, Is.False);
            Assert.That(equalsOpposite, Is.False);
        }

        [Test]
        public void EqualsISafeString_ParameterIsNullSafeBytesAndSelfIsEmpty_returnsTrue()
        {
            //Arrange
            using var sut = GetSut();
            ISafeString nullInstance = null;
            //Act
            var equals = sut.Equals(nullInstance);
            //Assert
            Assert.That(equals, Is.True);
        }

        [Test]
        public void EqualsISafeString_ParameterIsNullSafeBytesAndSelfIsNotEmpty_returnsFalse()
        {
            //Arrange
            using var sut = GetSut();
            sut.Append("I'm not empty!");
            ISafeString nullInstance = null;
            //Act
            var equals = sut.Equals(nullInstance);
            //Assert
            Assert.That(equals, Is.False);
        }

        [Test]
        public void EqualsString_WhenHoldingSameChars_returnsTrue()
        {
            //Arrange
            const char ch1 = 'a', ch2 = 'b';
            const string expected = "ab";
            using var sut = GetSut();
            sut.Append(ch1);
            sut.Append(ch2);
            //Act
            var equals = sut.Equals(expected);
            //Assert
            Assert.True(equals);
        }

        [Test]
        public void EqualsString_ParameterIsNullStringAndSelfIsEmpty_returnsTrue()
        {
            //Arrange
            using var sut = GetSut();
            string nullString = null;
            //Act
            var equals = sut.Equals(nullString);
            //Assert
            Assert.That(equals, Is.True);
        }

        [Test]
        public void EqualsString_ParameterIsNullStringAndSelfIsNotEmpty_returnsFalse()
        {
            //Arrange
            using var sut = GetSut();
            sut.Append("I'm not empty!");
            string nullString = null;
            //Act
            var equals = sut.Equals(nullString);
            //Assert
            Assert.That(equals, Is.False);
        }

        [Test]
        public void EqualsStringy_WhenHoldingDifferentChars_returnsFalse([Random(0, 125, 1)] byte i1,
            [Random(125, 256, 1)] byte i2)
        {
            //Arrange
            using var sut = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2;
            sut.Append(ch1);
            sut.Append(ch2);
            var text = $"{ch2}{ch1}";
            //Act
            var equals = sut.Equals(text);
            //Assert
            Assert.That(equals, Is.False);
        }

        [Test]
        public void GetHashCode_ForDistinctEmptyInstances_returnsTrue()
        {
            //Arrange
            using var sut = GetSut();
            var holdingSame = GetSut();
            var hashCode = sut.GetHashCode();
            var sameHashCode = holdingSame.GetHashCode();
            //Assert
            Assert.That(hashCode, Is.EqualTo(sameHashCode));
        }

        [Test]
        public void GetHashCode_ForDistinctObjectsHavingSameMultipleChars_returnsTrue(
            [Random(0, 256, 1)] int i1, [Random(0, 256, 1)] int i2, [Random(0, 256, 1)] int i3)
        {
            //Arrange
            using var sut = GetSut();
            using var holdingSame = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2, ch3 = (char) i3;
            sut.Append(ch1);
            sut.Append(ch2);
            sut.Append(ch3);
            holdingSame.Append(ch1);
            holdingSame.Append(ch2);
            holdingSame.Append(ch3);
            //Act
            var hashCode = sut.GetHashCode();
            var sameHashCode = holdingSame.GetHashCode();
            //Assert
            Assert.That(hashCode, Is.EqualTo(sameHashCode));
        }

        //** GetHashCode() **//
        [Test]
        public void GetHashCode_ForDistinctObjectsHavingSameSingleChar_returnsTrue([Random(0, 256, 1)] int i)
        {
            //Arrange
            using var sut = GetSut();
            using var holdingSame = GetSut();
            var ch = (char) i;
            sut.Append(ch);
            holdingSame.Append(ch);
            //Act
            var hashCode = sut.GetHashCode();
            var sameHashCode = holdingSame.GetHashCode();
            //Assert
            Assert.That(hashCode, Is.EqualTo(sameHashCode));
        }

        [Test]
        public void GetHashCode_ForTwoNonEqualObjects_returnsFalse(
            [Random(0, 256, 1)] int i1, [Random(0, 256, 1)] int i2, [Random(0, 256, 1)] int i3)
        {
            //Arrange
            using var sut = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2, ch3 = (char) i3;
            sut.Append(ch1);
            sut.Append(ch2);
            var holdingDifferent = GetSut();
            holdingDifferent.Append(ch3);
            //Act
            var hashCode = sut.GetHashCode();
            var hashCodeForDifferentObject = holdingDifferent.GetHashCode();
            //Assert
            Assert.That(hashCode, Is.Not.EqualTo(hashCodeForDifferentObject));
        }
    }
}