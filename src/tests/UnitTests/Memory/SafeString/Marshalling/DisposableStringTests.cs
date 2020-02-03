using System;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace SafeOrbit.Memory.SafeStringServices
{
    /// <seealso cref="DisposableString"/>
    [TestFixture]
    public class DisposableStringTests
    {
        [Test]
        public void String_AfterDisposingTheInstance_Throws()
        {
            // Arrange
            const string text = "testString";
            var handle = GCHandle.Alloc(text, GCHandleType.Pinned);
            var sut = new DisposableString(text, handle);

            // Act
            sut.Dispose();
            void ReadString() => _ = sut.String;

            // Assert
            Assert.Throws<ObjectDisposedException>(ReadString);
        }

        [Test]
        public void Dispose_PlainText_ClearsText()
        {
            // Arrange
            const string text = "testString";
            var handle = GCHandle.Alloc(text, GCHandleType.Pinned);
            var sut = new DisposableString(text, handle);

            // Act
            sut.Dispose();

            // Assert
            Assert.That(text, Has.All.EqualTo('\0'));
        }

        [Test]
        public void Dispose_EmptyStringNotAllocatedHandle_CanDispose()
        {
            // Arrange
            var sut = new DisposableString("", new GCHandle());

            // Act
            void Dispose() => sut.Dispose();

            // Assert
            Assert.DoesNotThrow(Dispose);
        }
    }
}
