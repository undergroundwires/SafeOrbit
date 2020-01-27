using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Fakes;

namespace SafeOrbit.Memory
{
    [TestFixture]
    public class SafeStringToStringMarshalerTests
    {
        [Test]
        public void SafeString_SettingNullSafeString_ThrowsArgumentNullException()
        {
            // Arrange
            var sut = GetSut();
            var nullSafeString = (ISafeString) null;

            // Act
            void SettingNullSafeString() => sut.SafeString = nullSafeString;

            // Assert
            Assert.That(SettingNullSafeString, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task SafeString_SettingDisposedSafeString_ThrowsObjectDisposedException()
        {
            // Arrange
            var sut = GetSut();
            var safeString = Stubs.Get<ISafeString>();
            await safeString.AppendAsync('a');

            // Act
            safeString.Dispose();
            void SettingDisposedSafeString() => sut.SafeString = safeString;

            // Assert
            Assert.That(SettingDisposedSafeString, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void SafeString_SettingEmptySafeString_ReturnsEmptyString()
        {
            // Arrange
            var sut = GetSut();
            var emptySafeString = Stubs.Get<ISafeString>();
            const string expected = "";

            // Act
            sut.SafeString = emptySafeString;
            var actual = sut.String;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task String_AfterSettingSafeString_ReturnsStringOfSafeString()
        {
            // Arrange
            var sut = GetSut();
            const string expected = "갿äÅ++¨¨'e";
            var safeString = Stubs.Get<ISafeString>();
            foreach (var ch in expected) 
                await safeString.AppendAsync(ch);
            
            // Act
            sut.SafeString = safeString;
            var actual = sut.String;
            
            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task String_AfterDisposingTheInstance_DoesNotReturnPlainText()
        {
            // Arrange
            var sut = GetSut();
            const string plainText = "testString";
            var safeString = Stubs.Get<ISafeString>();
            foreach (var ch in plainText.ToCharArray())
                await safeString.AppendAsync(ch);
            
            // Act
            sut.SafeString = safeString;
            sut.Dispose();
            var actual = sut.String;
            
            // Assert
            Assert.That(actual, Is.Not.EqualTo(plainText));
        }

        [Test]
        public async Task String_SettingStringToDisposedObject_Throws() 
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();
            const string expected = "갿äÅ++¨¨'e";
            var safeString = Stubs.Get<ISafeString>();
            foreach (var ch in expected)
                await safeString.AppendAsync(ch);
            
            // Act
            void SettingSafeString() => sut.SafeString = safeString;
            
            // Assert
            Assert.Throws<ObjectDisposedException>(SettingSafeString);
        }

        private static SafeStringToStringMarshaler GetSut() => new SafeStringToStringMarshaler();
    }
}