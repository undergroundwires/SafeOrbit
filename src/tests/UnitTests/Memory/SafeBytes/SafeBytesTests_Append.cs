using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SafeOrbit.Extensions;
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
        public async Task AppendAsyncByte_CleanInstance_ChangesHashCode()
        {
            // Arrange
            using var sut = GetSut();
            var oldCode = sut.GetHashCode();

            // Act
            await sut.AppendAsync(5);
            var newCode = sut.GetHashCode();

            // Assert
            Assert.AreNotEqual(oldCode, newCode);
        }

        [Test]
        public async Task AppendAsyncByte_InstanceWithBytes_ChangesHashCode()
        {
            // Arrange
            using var sut = GetSut();
            await sut.AppendManyAsync(new SafeMemoryStream(new byte[]{5, 10, 15, 31, 31}));
            var oldCode = sut.GetHashCode();

            // Act
            await sut.AppendAsync(5);
            var newCode = sut.GetHashCode();

            // Assert
            Assert.AreNotEqual(oldCode, newCode);
        }

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
        public async Task AppendAsyncByte_UnknownISafeBytes_AppendsToInternalCollection()
        {
            // Arrange
            const byte firstByte = 55, secondByte = 77;
            var expected = new SafeBytesFaker()
                .Provide();
            await expected.AppendAsync(firstByte);
            await expected.AppendAsync(secondByte);
            var collection = Stubs.Get<ISafeByteCollection>();
            using var sut = GetSut(collection: collection);

            // Act
            await sut.AppendAsync(expected);

            // Assert
            var actual = await collection.GetAllAsync();
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(firstByte, actual.ElementAt(0).RevealDecryptedByteAsync().Result);
            Assert.AreEqual(secondByte, actual.ElementAt(1).RevealDecryptedByteAsync().Result);
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
            var actual = await sut.RevealDecryptedByteAsync(0);
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
            var actual = await sut.RevealDecryptedBytesAsync();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task AppendAsyncISafeBytes_CleanInstance_ChangesHashCode()
        {
            // Arrange
            using var sut = GetSut();
            var oldCode = sut.GetHashCode();
            var toAppend = GetSut();
            await toAppend.AppendAsync(10);

            // Act
            await sut.AppendAsync(toAppend);
            var newCode = sut.GetHashCode();

            // Assert
            Assert.AreNotEqual(oldCode, newCode);
        }

        [Test]
        public async Task AppendAsyncISafeBytes_InstanceWithBytes_ChangesHashCode()
        {
            // Arrange
            using var sut = GetSut();
            var toAppend = GetSut();
            await toAppend.AppendAsync(10);
            var oldCode = sut.GetHashCode();

            // Act
            await sut.AppendAsync(toAppend);
            var newCode = sut.GetHashCode();

            // Assert
            Assert.AreNotEqual(oldCode, newCode);
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
            var actual = await sut.RevealDecryptedBytesAsync();
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
            var actual = await sut.RevealDecryptedBytesAsync();
            Assert.True(actual.SequenceEqual(expected));
        }

        [Test]
        public async Task AppendAsyncISafeBytes_SafeBytes_AppendsToInternalCollection()
        {
            // Arrange
            const byte firstByte = 55, secondByte = 77;
            var expected = Stubs.Get<ISafeBytes>();
            await expected.AppendAsync(firstByte);
            await expected.AppendAsync(secondByte);
            var collection = Stubs.Get<ISafeByteCollection>();
            using var sut = GetSut(collection: collection);

            // Act
            await sut.AppendAsync(expected);

            // Assert
            var actual = await collection.GetAllAsync();
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(firstByte, actual.ElementAt(0).RevealDecryptedByteAsync().Result);
            Assert.AreEqual(secondByte, actual.ElementAt(1).RevealDecryptedByteAsync().Result);
        }

        [Test]
        public async Task AppendManyAsync_CleanInstance_ChangesHashCode()
        {
            // Arrange
            using var sut = GetSut();
            var oldCode = sut.GetHashCode();

            // Act
            await sut.AppendManyAsync(new SafeMemoryStream(new byte[] { 31, 31 }));
            var newCode = sut.GetHashCode();

            // Assert
            Assert.AreNotEqual(oldCode, newCode);
        }

        [Test]
        public async Task AppendManyAsync_InstanceWithBytes_ChangesHashCode()
        {
            // Arrange
            using var sut = GetSut();
            await sut.AppendManyAsync(new SafeMemoryStream(new byte[] { 31, 31 }));
            var oldCode = sut.GetHashCode();

            // Act
            await sut.AppendManyAsync(new SafeMemoryStream(new byte[] { 31, 31 }));
            var newCode = sut.GetHashCode();

            // Assert
            Assert.AreNotEqual(oldCode, newCode);
        }

        [Test]
        public void AppendManyAsync_ForDisposedObject_ThrowsObjectDisposedException()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();

            // Act
            Task AppendManyAsync() => sut.AppendManyAsync(new SafeMemoryStream(new byte[] { 31 }));

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
            await sut.AppendManyAsync(new SafeMemoryStream(new byte[]{ 31, 69, 48 }));
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
            await sut.AppendManyAsync(new SafeMemoryStream(new []{ expected }));

            // Assert
            var actual = await sut.RevealDecryptedByteAsync(0);
            Assert.That(actual, Is.EqualTo(expected));
        }



        [Test]
        public async Task AppendManyAsync_ForCleanObject_CanAppendMultiple()
        {
            // Arrange
            byte[] expected = { 5, 10, 15, 31, 31 };
            using var sut = GetSut();

            // Act
            await sut.AppendManyAsync(new SafeMemoryStream(expected.CopyToNewArray()));

            // Assert
            var actual = await sut.RevealDecryptedBytesAsync();
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
            await sut.AppendManyAsync(new SafeMemoryStream(new byte[]{10,20}));

            // Assert
            var actual = await sut.RevealDecryptedBytesAsync();
            Assert.True(actual.SequenceEqual(expected));
        }


        [Test]
        public async Task AppendManyAsync_MultipleBytes_AppendsToInternalCollection()
        {
            // Arrange
            const byte firstByte = 55, secondByte = 77;
            var expected = Stubs.Get<ISafeBytes>();
            var stream = new SafeMemoryStream(new [] { firstByte, secondByte });
            await expected.AppendManyAsync(stream);
            var collection = Stubs.Get<ISafeByteCollection>();
            using var sut = GetSut(collection: collection);

            // Act
            await sut.AppendAsync(expected);

            // Assert
            var actual = await collection.GetAllAsync();
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(firstByte, actual.ElementAt(0).RevealDecryptedByteAsync().Result);
            Assert.AreEqual(secondByte, actual.ElementAt(1).RevealDecryptedByteAsync().Result);
        }
    }
}
