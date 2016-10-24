using System;
using NUnit.Framework;
using SafeOrbit.UnitTests;

namespace SafeOrbit.Memory
{
    [TestFixture]
    public class SafeStringToStringMarshalerTests
    {
        [Test]
        public void SafeString_SettingNullSafeString_throwsArgumentNullException()
        {
            //Arrange
            var sut = GetSut();
            var nullSafeString = (ISafeString)null;
            //Act
            TestDelegate settingNullSafeString = () => sut.SafeString = nullSafeString;
            //Assert
            Assert.That(settingNullSafeString, Throws.TypeOf<ArgumentNullException>());
        }
        [Test]
        public void SafeString_SettingDisposedSafeString_throwsObjectDisposedException()
        {
            //Arrange
            var sut = GetSut();
            var safeString = Stubs.Get<ISafeString>().AppendAndReturnDeepClone('a');
            //Act
            safeString.Dispose();
            TestDelegate settingDisposedSafeString = () => sut.SafeString = safeString;
            //Assert
            Assert.That(settingDisposedSafeString, Throws.TypeOf<ObjectDisposedException>());
        }
        [Test]
        public void SafeString_SettingEmptySafeString_returnsEmptyString()
        {
            //Arrange
            var sut = GetSut();
            var emptySafeString = Stubs.Get<ISafeString>();
            var expected = "";
            //Act
            sut.SafeString = emptySafeString;
            var actual = sut.String;
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test]
        public void String_AfterSettingSafeString_returnsStringOfSafeString()
        {
            //Arrange
            var sut = GetSut();
            const string expected = "갿äÅ++¨¨'e";
            var safeString = Stubs.Get<ISafeString>();
            foreach (var ch in expected.ToCharArray())
            {
                safeString.Append(ch);
            }
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
            var safeString = Stubs.Get<ISafeString>();
            foreach (var ch in plainText.ToCharArray())
            {
                safeString.Append(ch);
            }
            //Act
            sut.SafeString = safeString;
            sut.Dispose();
            var actual = sut.String;
            //assert
            Assert.That(actual, Is.Not.EqualTo(plainText));
        }
        [Test]
        public void String_AfterSettingSafeStringToDisposedObject_returnsStringOfSafeString()
        {
            //Arrange
            var sut = GetSut();
            sut.Dispose();
            const string expected = "갿äÅ++¨¨'e";
            var safeString = Stubs.Get<ISafeString>();
            foreach (var ch in expected.ToCharArray())
            {
                safeString.Append(ch);
            }
            //Act
            sut.SafeString = safeString;
            var actual = sut.String;
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
        private static SafeStringToStringMarshaler GetSut() => new SafeStringToStringMarshaler();
    }
}