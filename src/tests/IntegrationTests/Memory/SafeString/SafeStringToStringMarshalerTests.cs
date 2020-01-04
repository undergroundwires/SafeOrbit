using NUnit.Framework;

namespace SafeOrbit.Memory
{
    [TestFixture]
    public class SafeStringToStringMarshalerTests
    {
        [Test]
        public void String_AfterSettingSafeString_returnsStringOfSafeString()
        {
            //Arrange
            var sut = GetSut();
            const string expected = "갿äÅ++¨¨'e";
            var safeString = new SafeString();
            foreach (var ch in expected.ToCharArray()) safeString.Append(ch);
            //Act
            sut.SafeString = safeString;
            var actual = sut.String;
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void String_AfterDisposingTheInstance_doesNotReturnPlainText()
        {
            //Arrange
            var sut = GetSut();
            const string plainText = "testString";
            var safeString = new SafeString();
            foreach (var ch in plainText.ToCharArray()) safeString.Append(ch);
            //Act
            sut.SafeString = safeString;
            sut.Dispose();
            var actual = sut.String;
            //assert
            Assert.That(actual, Is.Not.EqualTo(plainText));
        }

        private static SafeStringToStringMarshaler GetSut() => new SafeStringToStringMarshaler();
    }
}