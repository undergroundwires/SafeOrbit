using System;
using NUnit.Framework;

namespace SafeOrbit.Common
{
    [TestFixture]
    public class DisposableBaseTests
    {
        [Test]
        public void Dispose_Called_InvokesDisposeMethods()
        {
            // Arrange
            var cut = new DisposableUnderTest();

            // Act
            cut.Dispose();

            // Assert
            Assert.True(cut.IsDisposeManagedCalled);
            Assert.True(cut.IsDisposeUnmanagedCalled);
        }


        [Test]
        public void Dispose_NotCalled_DisposeMethodsAreNotInvoked()
        {
            // Arrange
            var cut = new DisposableUnderTest();

            // Assert
            Assert.False(cut.IsDisposeManagedCalled);
            Assert.False(cut.IsDisposeUnmanagedCalled);
        }

        [Test]
        public void Dispose_CalledMultipleTimes_DoesNotThrow()
        {
            // Arrange
            var sut = new DisposableUnderTest();

            // Act
            void DisposeManyTimes()
            {
                for (var i = 0; i < 15; i++)
                    sut.Dispose();
            }

            // Assert
            Assert.DoesNotThrow(DisposeManyTimes);
        }

        [Test]
        public void Dispose_CalledMultipleTimes_DisposesResourcesOnce()
        {
            // Arrange
            var sut = new DisposableUnderTest();

            // Act
            sut.Dispose();
            sut.Dispose();

            // Assert
            Assert.AreEqual(1, sut.DisposeCalledCount);
        }

        [Test]
        public void IsDisposed_Disposed_ReturnsTrue()
        {
            // Arrange
            var sut = new DisposableUnderTest();

            // Act
            sut.Dispose();

            // Assert
            Assert.True(sut.IsDisposed);
        }

        [Test]
        public void IsDisposed_NotDisposed_ReturnsFalse()
        {
            // Arrange
            using var sut = new DisposableUnderTest();

            // Act
            var actual = sut.IsDisposed;

            // Assert
            Assert.False(actual);
        }


        [Test]
        public void ThrowIfDisposed_Disposed_Throws()
        {
            // Arrange
            var sut = new DisposableUnderTest();

            // Act
            sut.Dispose();
            void ThrowIfDisposed() => sut.ThrowIfDisposed();

            // Assert
            Assert.Throws<ObjectDisposedException>(ThrowIfDisposed);
        }

        [Test]
        public void ThrowIfDisposed_NotDisposed_DoesNotThrow()
        {
            // Arrange
            using var sut = new DisposableUnderTest();

            // Act
            void ThrowIfDisposed() => sut.ThrowIfDisposed();

            // Assert
            Assert.DoesNotThrow(ThrowIfDisposed);
        }


        private class DisposableUnderTest : DisposableBase
        {
            public int DisposeCalledCount { get; private set; }
            public bool IsDisposeManagedCalled { get; private set; }
            public bool IsDisposeUnmanagedCalled { get; private set; }

            public new void ThrowIfDisposed() => base.ThrowIfDisposed();

            protected override void DisposeManagedResources()
            {
                IsDisposeManagedCalled = true;
                DisposeCalledCount++;
            }

            protected override void DisposeUnmanagedResources()
            {
                IsDisposeUnmanagedCalled = true;
            }
        }
    }
}
