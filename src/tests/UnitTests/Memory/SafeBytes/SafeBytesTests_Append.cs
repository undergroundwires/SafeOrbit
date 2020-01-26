using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.SafeBytesServices;
using SafeOrbit.Memory.SafeBytesServices.Collection;
using SafeOrbit.Memory.SafeBytesServices.Factory;

namespace SafeOrbit.Memory
{
    [TestFixture]
    public partial class SafeBytesTests
    {

        [Test]
        public async Task AppendAsyncByte_SingleByte_IncreasesLength()
        {
            // Arrange
            const int expected = 1;
            using var sut = GetSut();
            await sut.AppendAsync(5);

            // Act
            var actual = sut.Length;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task AppendAsyncByte_MultipleBytes_IncreasesLength()
        {
            // Arrange
            const int expected = 3;
            using var sut = GetSut();
            await sut.AppendAsync(5);
            await sut.AppendAsync(5);
            await sut.AppendAsync(5);

            // Act
            var actual = sut.Length;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task AppendAsyncByte_PlainByte_AppendsFromFactory()
        {
            // Arrange
            const byte @byte = 55;
            var expected = new Mock<ISafeByte>();
            var factoryMock = new Mock<ISafeByteFactory>();
            factoryMock.Setup(f => f.GetByByteAsync(@byte))
                .ReturnsAsync(expected.Object);
            var collection = new Mock<ISafeByteCollection>();
            collection.Setup(c => c.AppendAsync(expected.Object))
                .Verifiable();
            collection.Setup(c => c.AppendAsync(It.IsAny<ISafeByte>()));
            using var sut = GetSut(collection: collection.Object, factory: factoryMock.Object);

            // Act
            await sut.AppendAsync(@byte);

            // Assert
            collection.Verify(c => c.AppendAsync(expected.Object));
        }

        [Test]
        public async Task AppendAsyncByte_UnknownISafeBytes_Appends()
        {
            // Arrange
            const byte firstByte = 55, secondByte = 77;
            var expected = new SafeBytesFaker()
                .Provide();
            await expected.AppendAsync(firstByte);
            await expected.AppendAsync(secondByte);
            var collection = new Mock<ISafeByteCollection>();
            collection.Setup(c => c.AppendAsync(It.IsAny<ISafeByte>()))
                .Verifiable();
            using var sut = GetSut(collection: collection.Object);

            // Act
            await sut.AppendAsync(expected);

            // Assert
            collection.Verify(c => c.AppendAsync(It.Is<ISafeByte>(b => b.GetAsync().Result == firstByte)), Times.Once);
            collection.Verify(c => c.AppendAsync(It.Is<ISafeByte>(b => b.GetAsync().Result == secondByte)), Times.Once);
        }

        [Test]
        public void AppendAsyncByte_ForDisposedObject_ThrowsObjectDisposedException([Random(0, 256, 1)] byte b)
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();

            // Act
            Task AppendingByte() => sut.AppendAsync(b);

            // Assert
            Assert.That(AppendingByte, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public async Task AppendAsyncByte_ForCleanObject_CanAppendSingle()
        {
            // Arrange
            const byte expected = 5;
            using var sut = GetSut();

            // Act
            await sut.AppendAsync(expected);

            // Assert
            var actual = await sut.GetByteAsync(0);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task AppendAsyncByte_ForCleanObject_CanAppendMultiple()
        {
            // Arrange
            byte[] expected = { 5, 10, 15, 31, 31 };
            using var sut = GetSut();

            // Act
            foreach (var @byte in expected)
                await sut.AppendAsync(@byte);

            // Assert
            var actual = await sut.ToByteArrayAsync();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task AppendAsyncISafeBytes_MultipleBytesOnCleanInstance_AppendsAsExpected()
        {
            // Arrange
            byte[] expected = { 3, 5, 10, 20 };
            using var sut = GetSut();
            var toAppend = GetSut();
            foreach (var @byte in expected)
                await toAppend.AppendAsync(@byte);

            // Act
            await sut.AppendAsync(toAppend);

            // Assert
            var actual = await sut.ToByteArrayAsync();
            Console.WriteLine($"Actual: ${string.Join(",", actual)}{Environment.NewLine}" +
                              $"Expected: ${string.Join(",", expected)}");
            Assert.True(actual.SequenceEqual(expected));
        }

        [Test]
        public async Task AppendAsyncISafeBytes_SingleSafeByte_IncreasesLength()
        {
            // Arrange
            const int expected = 1;
            using var sut = GetSut();
            var toAppend = GetSut();
            await toAppend.AppendAsync(10);

            // Act
            await sut.AppendAsync(toAppend);
            var actual = sut.Length;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task AppendAsyncISafeBytes_MultipleBytesOnInstanceWithExistingBytes_AppendsAsExpected()
        {
            // Arrange
            byte[] expected = { 3, 5, 10, 20 };
            using var sut = GetSut();
            await sut.AppendAsync(3);
            await sut.AppendAsync(5);
            var toAppend = GetSut();
            await toAppend.AppendAsync(10);
            await toAppend.AppendAsync(20);

            // Act
            await sut.AppendAsync(toAppend);

            // Assert
            var actual = await sut.ToByteArrayAsync();
            Assert.True(actual.SequenceEqual(expected));
        }
        [Test]
        public async Task AppendAsyncISafeBytes_SafeBytes_AppendsFromFactory()
        {
            // Arrange
            const byte firstByte = 55, secondByte = 77;
            var expected = Stubs.Get<ISafeBytes>();
            await expected.AppendAsync(firstByte);
            await expected.AppendAsync(secondByte);
            var collection = new Mock<ISafeByteCollection>();
            collection.Setup(c => c.AppendAsync(It.IsAny<ISafeByte>()))
                .Verifiable();
            using var sut = GetSut(collection: collection.Object);

            // Act
            await sut.AppendAsync(expected);

            // Assert
            collection.Verify(c => c.AppendAsync(It.Is<ISafeByte>(b => b.GetAsync().Result == 55)), Times.Once);
            collection.Verify(c => c.AppendAsync(It.Is<ISafeByte>(b => b.GetAsync().Result == 77)), Times.Once);
        }

        [Test]
        public void AppendManyAsync_ForDisposedObject_ThrowsObjectDisposedException()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();

            // Act
            Task AppendManyAsync() => sut.AppendManyAsync(GetStream(31));

            // Assert
            Assert.That(AppendManyAsync, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public async Task AppendManyAsync_AFewBytes_IncreasesLength()
        {
            // Arrange
            const int expected = 3;
            using var sut = GetSut();

            // Act
            await sut.AppendManyAsync(GetStream(31, 69, 48));
            var actual = sut.Length;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task AppendManyAsync_ForCleanObject_CanAppendSingle()
        {
            // Arrange
            const byte expected = 5;
            using var sut = GetSut();


            // Act
            await sut.AppendManyAsync(GetStream(expected));

            // Assert
            var actual = await sut.GetByteAsync(0);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task AppendManyAsync_ForCleanObject_CanAppendMultiple()
        {
            // Arrange
            byte[] expected = { 5, 10, 15, 31, 31 };
            using var sut = GetSut();

            // Act
            await sut.AppendManyAsync(GetStream(expected));

            // Assert
            var actual = await sut.ToByteArrayAsync();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task AppendManyAsync_MultipleBytesOnInstanceWithExistingBytes_AppendsAsExpected()
        {
            // Arrange
            byte[] expected = { 3, 5, 10, 20 };
            using var sut = GetSut();
            await sut.AppendAsync(3);
            await sut.AppendAsync(5);

            // Act
            await sut.AppendManyAsync(GetStream(10, 20));

            // Assert
            var actual = await sut.ToByteArrayAsync();
            Assert.True(actual.SequenceEqual(expected));
        }
    }
}
