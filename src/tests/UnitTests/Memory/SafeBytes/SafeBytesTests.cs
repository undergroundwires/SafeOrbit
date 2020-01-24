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
using SafeOrbit.Tests;

namespace SafeOrbit.Memory
{
    /// <seealso cref="SafeBytes" />
    /// <seealso cref="ISafeBytes" />
    public class SafeBytesTests : TestsFor<ISafeBytes>
    {
        [Test]
        public void IsNullOrEmpty_ForNullSafeBytesObject_returnsTrue()
        {
            // Arrange
            ISafeBytes nullBytes = null;

            // Act
            var isNull = SafeBytes.IsNullOrEmpty(nullBytes);

            // Assert
            Assert.That(isNull, Is.True);
        }

        [Test]
        public async Task IsNullOrEmpty_ForDisposedSafeBytesObject_returnsTrue([Random(0, 256, 1)] byte b)
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync(b);
            sut.Dispose();

            // Act
            var isNull = SafeBytes.IsNullOrEmpty(sut);

            // Assert
            Assert.That(isNull, Is.True);
        }

        [Test]
        public async Task IsNullOrEmpty_ForObjectHoldingSingleByte_returnsFalse([Random(0, 256, 1)] byte b)
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync(b);

            // Act
            var isEmpty = SafeBytes.IsNullOrEmpty(sut);

            // Assert
            Assert.That(isEmpty, Is.False);
        }

        [Test]
        public async Task IsNullOrEmpty_ForObjectHoldingMultipleBytes_returnsFalse([Random(0, 256, 1)] byte b1,
            [Random(0, 256, 1)] byte b2, [Random(0, 256, 1)] byte b3)
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync(b1);
            await sut.AppendAsync(b2);
            await sut.AppendAsync(b3);

            // Act
            var isNull = SafeBytes.IsNullOrEmpty(sut);

            // Assert
            Assert.That(isNull, Is.False);
        }

        [Test]
        public void IsNullOrEmpty_ForNewSafeBytesInstance_returnsTrue()
        {
            // Arrange
            var sut = GetSut();

            // Act
            var isEmpty = SafeBytes.IsNullOrEmpty(sut);

            // Assert
            Assert.That(isEmpty, Is.True);
        }

        [Test]
        public void IsDisposed_ForNewSafeBytesInstance_returnsFalse()
        {
            // Arrange
            using var sut = GetSut();

            // Act
            var isDisposed = sut.IsDisposed;

            // Assert
            Assert.That(isDisposed, Is.False);
        }

        [Test]
        public void IsDisposed_ForDisposedInstance_returnsFalse()
        {
            // Arrange
            using var sut = GetSut();
            sut.Dispose();

            // Act
            var isDisposed = sut.IsDisposed;

            // Assert
            Assert.That(isDisposed, Is.True);
        }

        [Test]
        public void Length_ForNewEmptyInstance_returnsZero()
        {
            // Arrange
            const int expected = 0;
            using var sut = GetSut();

            // Act
            var isDisposed = sut.Length;

            // Assert
            Assert.That(isDisposed, Is.EqualTo(expected));
        }

        [Test]
        public async Task Length_SingleByte_returnsOne()
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
        public async Task Length_MultipleBytes_returnsExpected()
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
        public async Task AppendAsync_PlainByte_AppendsFromFactory()
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
        public async Task AppendAsync_UnknownISafeBytes_Appends()
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
        public async Task AppendAsync_SafeBytes_AppendsFromFactory()
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
        public void AppendAsyncByte_ForDisposedObject_throwsObjectDisposedException([Random(0, 256, 1)] byte b)
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
        public async Task AppendAsyncByte_ForCleanObject_canAppendSingle()
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
        public async Task AppendAsyncByte_ForCleanObject_canAppendMultiple()
        {
            // Arrange
            byte[] expected = {5, 10, 15, 31, 31};
            using var sut = GetSut();

            // Act
            foreach (var @byte in expected)
                await sut.AppendAsync(@byte);

            // Assert
            var actual = await sut.ToByteArrayAsync();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task AppendAsyncSafeBytes_MultipleBytesOnCleanInstance_appendsAsExpected()
        {
            // Arrange
            byte[] expected = {3, 5, 10, 20};
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
        public async Task AppendAsyncSafeBytes_MultipleBytesOnInstanceWithExistingBytes_appendsAsExpected()
        {
            // Arrange
            byte[] expected = {3, 5, 10, 20};
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
        public void GetByteAsync_OnEmptyInstance_throwsInvalidOperationException()
        {
            // Arrange
            using var sut = GetSut();
            const int position = 0;

            // Act
            Task CallingOnEmptyInstance() => sut.GetByteAsync(position);
            
            // Assert
            Assert.ThrowsAsync<InvalidOperationException>(CallingOnEmptyInstance);
        }

        [Test]
        public async Task GetByteAsync_ForExistingByteAtStart_retrievesAsExpected()
        {
            // Arrange
            const byte expected = 55;
            using var sut = GetSut();
            await sut.AppendAsync(expected);
            
            // Act
            var actual = await sut.GetByteAsync(0);
            
            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetByteAsync_ForExistingByteAtEnd_retrievesAsExpected()
        {
            // Arrange
            const byte expected = 55;
            using var sut = GetSut();
            await sut.AppendAsync(22);
            await sut.AppendAsync(expected);

            // Act
            var actual = await sut.GetByteAsync(1);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetByteAsync_ForExistingByteInTheMiddle_retrievesAsExpected()
        {
            // Arrange
            const byte expected = 55;
            using var sut = GetSut();
            await sut.AppendAsync(22);
            await sut.AppendAsync(expected);
            await sut.AppendAsync(31);

            // Act
            var actual = await sut.GetByteAsync(1);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task ToByteArrayAsync_ForEmptyInstance_returnsEmptyArray()
        {
            // Arrange
            using var sut = GetSut();
            
            // Act
            var actual = await sut.ToByteArrayAsync();
            
            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Has.Length.Zero);
        }

        [Test]
        public async Task ToByteArrayAsync_ForInstanceWithSingleByte_returnsExpected()
        {
            // Arrange
            byte[] expected = {5};
            using var sut = GetSut();
            await sut.AppendAsync(5);

            // Act
            var actual = await sut.ToByteArrayAsync();

            // Assert
            Assert.That(actual.SequenceEqual(expected));
        }

        [Test]
        public async Task ToByteArrayAsync_MultipleBytesAddedOneByOne_ReturnsExpected()
        {
            // Arrange
            byte[] expected = { 5, 10, 15 };
            using var sut = GetSut();

            // Act
            foreach (var @byte in expected)
                await sut.AppendAsync(@byte);
            var actual = await sut.ToByteArrayAsync();

            // Assert
            Assert.That(actual.SequenceEqual(expected));
        }

        [Test]
        public async Task ToByteArrayAsync_MultipleBytesAddedAtOnce_ReturnsExpected()
        {
            // Arrange
            byte[] expected = { 5, 10, 15 };
            using var sut = GetSut();
            var stream = new SafeMemoryStream();
            stream.Write(expected.CopyToNewArray(), 0, expected.Length);

            // Act
            await sut.AppendManyAsync(stream);
            var actual = await sut.ToByteArrayAsync();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public async Task DeepCloneAsync_EmptyInstance_returnsDifferentEmptyInstance()
        {
            // Arrange
            var sut = GetSut();

            // Act
            var clone = await sut.DeepCloneAsync()
                .ConfigureAwait(false);

            // Assert
            Assert.False(ReferenceEquals(sut, clone));
            Assert.True(await sut.EqualsAsync(clone));
            Assert.True(await clone.EqualsAsync(sut));
        }

        [Test]
        public void DeepCloneAsync_Disposed_Throws()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();

            // Act
            Task DeepClone() => sut.DeepCloneAsync();

            // Assert
            Assert.ThrowsAsync<ObjectDisposedException>(DeepClone);
        }

        [Test]
        public async Task DeepCloneAsync_WithExistingBytes_returnsDifferentEqualInstance()
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync(55);
            await sut.AppendAsync(31);

            // Act
            var clone = await sut.DeepCloneAsync();

            // Assert
            Assert.False(ReferenceEquals(sut, clone));
            Assert.True(await sut.EqualsAsync(clone));
            Assert.True(await clone.EqualsAsync(sut));
        }

        [Test]
        public async Task EqualsAsync_SafeBytes_DifferentLength_returnsFalse()
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync(1);
            await sut.AppendAsync(1);
            var other = GetSut();
            await other.AppendAsync(1);

            // Act
            var equality = await sut.EqualsAsync(other);
            var equalityOpposite = await other.EqualsAsync(sut);

            // Assert
            Assert.False(equality);
            Assert.False(equalityOpposite);
        }

        [Test]
        public async Task EqualsAsync_ClearBytes_DifferentLength_returnsFalse()
        {
            // Arrange
            using var sut = GetSut();
            await sut.AppendAsync(1);
            var other = new byte[] {1, 1};

            // Act
            var actual = await sut.EqualsAsync(other);

            // Assert
            Assert.False(actual);
        }

        [Test]
        public async Task EqualsAsync_SafeBytes_SameLengthDifferentBytes_returnsFalse()
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync(5);
            await sut.AppendAsync(10);
            var other = GetSut();
            await other.AppendAsync(3);
            await sut.AppendAsync(10);

            // Act
            var equality = await sut.EqualsAsync(other);
            var equalityOpposite = await other.EqualsAsync(sut);

            // Assert
            Assert.False(equality);
            Assert.False(equalityOpposite);
        }

        [Test]
        public async Task EqualsAsync_ClearBytes_SameLengthDifferentBytes_returnsFalse()
        {
            // Arrange
            using var sut = GetSut();
            await sut.AppendAsync(5);
            await sut.AppendAsync(10);
            var other = new byte[] {3, 10};

            // Act
            var actual = await sut.EqualsAsync(other);

            // Assert
            Assert.False(actual);
        }

        [Test]
        public async Task EqualsAsync_SafeBytes_SameBytes_returnsTrue()
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync(5);
            await sut.AppendAsync(10);
            var other = GetSut();
            await other.AppendAsync(5);
            await other.AppendAsync(10);

            // Act
            var equality = await sut.EqualsAsync(other);
            var equalityOpposite = await other.EqualsAsync(sut);

            // Assert
            Assert.True(equality);
            Assert.True(equalityOpposite);
        }

        [Test]
        public async Task EqualsAsync_ClearBytes_SameBytes_returnsTrue()
        {
            // Arrange
            using var sut = GetSut();
            await sut.AppendAsync(5);
            await sut.AppendAsync(10);
            var other = new byte[] {5, 10};

            // Act
            var actual = await sut.EqualsAsync(other);

            // Assert
            Assert.True(actual);
        }

        [Test]
        public void GetHashCode_BothEmpty_returnsSame()
        {
            // Arrange
            using var sut = GetSut();
            using var other = GetSut();

            // Act
            var hashSut = sut.GetHashCode();
            var hashOther = other.GetHashCode();

            // Assert
            Assert.That(hashSut, Is.EqualTo(hashOther));
        }

        [Test]
        public async Task GetHashCode_SameNonEmptyBytes_returnsSame()
        {
            // Arrange
            using var sut = GetSut();
            await sut.AppendAsync(3);
            await sut.AppendAsync(5);
            using var other = GetSut();
            await other.AppendAsync(3);
            await other.AppendAsync(5);

            // Act
            var hashSut = sut.GetHashCode();
            var hashOther = other.GetHashCode();

            // Assert
            Assert.That(hashSut, Is.EqualTo(hashOther));
        }

        [Test]
        public async Task GetHashCode_DifferentBytesSameLength_returnsFalse()
        {
            // Arrange
            using var sut = GetSut();
            await sut.AppendAsync(3);
            using var other = GetSut();
            await other.AppendAsync(5);

            // Act
            var hashSut = sut.GetHashCode();
            var hashOther = other.GetHashCode();

            // Assert
            Assert.That(hashSut, Is.Not.EqualTo(hashOther));
        }

        [Test]
        public async Task GetHashCode_DifferentBytesDifferentLength_returnsFalse()
        {
            // Arrange
            using var sut = GetSut();
            await sut.AppendAsync(1);
            using var other = GetSut();
            await other.AppendAsync(1);
            await other.AppendAsync(1);

            // Act
            var hashSut = sut.GetHashCode();
            var hashOther = other.GetHashCode();

            // Assert
            Assert.That(hashSut, Is.Not.EqualTo(hashOther));
        }

        [Test]
        public void Dispose_AfterInvocation_setsIsDisposedToTrue()
        {
            // Arrange
            var sut = GetSut();

            // Act
            sut.Dispose();

            // Assert
            Assert.True(sut.IsDisposed);
        }

        [Test]
        public void Dispose_AfterInvocation_disposesInnerCollection()
        {
            // Arrange
            var mock = new Mock<ISafeByteCollection>(MockBehavior.Strict);
            mock.Setup(m => m.Dispose()).Verifiable();
            var sut = GetSut(collection: mock.Object);

            // Act
            sut.Dispose();

            // Assert
            mock.Verify(m => m.Dispose(), Times.Once);
        }

        [Test]
        public void IsDisposed_OnEmptyInstance_returnsFalse()
        {
            // Arrange & act
            using var sut = GetSut();

            // Assert
            Assert.False(sut.IsDisposed);
        }

        [Test]
        public async Task IsDisposed_NonEmptyInstance_returnsFalse()
        {
            // Arrange & act
            using var sut = GetSut();
            await sut.AppendAsync(5);
            await sut.AppendAsync(10);

            // Assert
            Assert.False(sut.IsDisposed);
        }

        protected override ISafeBytes GetSut() => GetSut();

        private static ISafeBytes GetSut(ISafeByteCollection collection = null, ISafeByteFactory factory = null)
        {
            return new SafeBytes(
                factory ?? Stubs.Get<ISafeByteFactory>(),
                Stubs.GetFactory<ISafeBytes>(),
                Stubs.GetFactory(collection));
        }
    }
}