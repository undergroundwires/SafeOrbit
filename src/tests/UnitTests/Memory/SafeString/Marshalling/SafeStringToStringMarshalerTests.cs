using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Fakes;

namespace SafeOrbit.Memory.SafeStringServices
{
    [TestFixture]
    public class SafeStringToStringMarshalerTests
    {
        [Test]
        public void MarshalAsync_NullSafeString_ThrowsArgumentNullException()
        {
            // Arrange
            var sut = GetSut();
            var nullSafeString = (ISafeString) null;

            // Act
            Task SetNullSafeString() => sut.MarshalAsync(nullSafeString);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(SetNullSafeString);
        }

        [Test]
        public async Task MarshalAsync_DisposedSafeString_ThrowsObjectDisposedException()
        {
            // Arrange
            var sut = GetSut();
            var safeString = Stubs.Get<ISafeString>();
            await safeString.AppendAsync('a');

            // Act
            safeString.Dispose();
            Task SetDisposedSafeString() => sut.MarshalAsync(safeString);

            // Assert
            Assert.ThrowsAsync<ObjectDisposedException>(SetDisposedSafeString);
        }

        [Test]
        public async Task MarshalAsync_SettingEmptySafeString_ReturnsEmptyString()
        {
            // Arrange
            var sut = GetSut();
            var emptySafeString = Stubs.Get<ISafeString>();
            const string expected = "";

            // Act
            var text = await sut.MarshalAsync(emptySafeString);
            var actual = text.String;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task MarshalAsync_InitializedInstance_ReturnsStringOfSafeString()
        {
            // Arrange
            var sut = GetSut();
            const string expected = "갿äÅ++¨¨'e";
            var safeString = Stubs.Get<ISafeString>();
            await safeString.AppendAsync(expected);

            // Act
            var text = await sut.MarshalAsync(safeString);
            var actual = text.String;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        private static SafeStringToStringMarshaler GetSut() => new SafeStringToStringMarshaler();
    }
}