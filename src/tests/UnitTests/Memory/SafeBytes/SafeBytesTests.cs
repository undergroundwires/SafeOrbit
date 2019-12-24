using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using SafeOrbit.Cryptography.Random;
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
            //Arrange
            ISafeBytes nullBytes = null;
            //Act
            var isNull = SafeBytes.IsNullOrEmpty(nullBytes);
            //Assert
            Assert.That(isNull, Is.True);
        }

        [Test]
        public void IsNullOrEmpty_ForDisposedSafeBytesObject_returnsTrue([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            sut.Append(b);
            sut.Dispose();
            //Act
            var isNull = SafeBytes.IsNullOrEmpty(sut);
            //Assert
            Assert.That(isNull, Is.True);
        }

        [Test]
        public void IsNullOrEmpty_ForObjectHoldingSingleByte_returnsFalse([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            sut.Append(b);
            //Act
            var isEmpty = SafeBytes.IsNullOrEmpty(sut);
            //Assert
            Assert.That(isEmpty, Is.False);
        }

        [Test]
        public void IsNullOrEmpty_ForObjectHoldingMultipleBytes_returnsFalse([Random(0, 256, 1)] byte b1,
            [Random(0, 256, 1)] byte b2, [Random(0, 256, 1)] byte b3)
        {
            //Arrange
            var sut = GetSut();
            sut.Append(b1);
            sut.Append(b2);
            sut.Append(b3);
            //Act
            var isNull = SafeBytes.IsNullOrEmpty(sut);
            //Assert
            Assert.That(isNull, Is.False);
        }

        [Test]
        public void IsNullOrEmpty_ForNewSafeBytesInstance_returnsTrue()
        {
            //Arrange
            var sut = GetSut();
            //Act
            var isEmpty = SafeBytes.IsNullOrEmpty(sut);
            //Assert
            Assert.That(isEmpty, Is.True);
        }

        [Test]
        public void IsDisposed_ForNewSafeBytesInstance_returnsFalse()
        {
            //Arrange
            using var sut = GetSut();
            //Act
            var isDisposed = sut.IsDisposed;
            //Assert
            Assert.That(isDisposed, Is.False);
        }

        [Test]
        public void IsDisposed_ForDisposedInstance_returnsFalse()
        {
            //Arrange
            using var sut = GetSut();
            sut.Dispose();
            //Act
            var isDisposed = sut.IsDisposed;
            //Assert
            Assert.That(isDisposed, Is.True);
        }

        [Test]
        public void Length_ForNewEmptyInstance_returnsZero()
        {
            //Arrange
            const int expected = 0;
            using var sut = GetSut();
            //Act
            var isDisposed = sut.Length;
            //Assert
            Assert.That(isDisposed, Is.EqualTo(expected));
        }

        [Test]
        public void Length_SingleByte_returnsOne()
        {
            //Arrange
            const int expected = 1;
            using var sut = GetSut();
            sut.Append(5);
            //Act
            var actual = sut.Length;
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Length_MultipleBytes_returnsExpected()
        {
            //Arrange
            const int expected = 3;
            using var sut = GetSut();
            sut.Append(5);
            sut.Append(5);
            sut.Append(5);
            //Act
            var actual = sut.Length;
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test]
        public void Append_PlainByte_AppendsFromFactory()
        {
            //Arrange
            const byte @byte = 55;
            var expected = new Mock<ISafeByte>();
            var factoryMock = new Mock<ISafeByteFactory>();
            factoryMock.Setup(f => f.GetByByte(@byte))
                .Returns(expected.Object);
            var collection = new Mock<ISafeByteCollection>();
            collection.Setup(c => c.Append(expected.Object))
                .Verifiable();
            collection.Setup(c => c.Append(It.IsAny<ISafeByte>()));
            using var sut = GetSut(collection: collection.Object, factory: factoryMock.Object);
            //Act
            sut.Append(@byte);
            //Assert
            collection.Verify(c => c.Append(expected.Object));
        }

        [Test]
        public void Append_UnknownISafeBytes_Appends()
        {
            //Arrange
            const byte firstByte = 55, secondByte = 77;
            var expected = new SafeBytesFaker()
                .Provide();
            expected.Append(firstByte);
            expected.Append(secondByte);
            var collection = new Mock<ISafeByteCollection>();
            collection.Setup(c => c.Append(It.IsAny<ISafeByte>()))
                .Verifiable();
            using var sut = GetSut(collection: collection.Object);
            //Act
            sut.Append(expected);
            //Assert
            collection.Verify(c => c.Append(It.Is<ISafeByte>(b => b.Get() == firstByte)), Times.Once);
            collection.Verify(c => c.Append(It.Is<ISafeByte>(b => b.Get() == secondByte)), Times.Once);
        }

        [Test]
        public void Append_SafeBytes_AppendsFromFactory()
        {
            //Arrange
            const byte firstByte = 55, secondByte = 77;
            var expected = Stubs.Get<ISafeBytes>();
            expected.Append(firstByte);
            expected.Append(secondByte);
            var collection = new Mock<ISafeByteCollection>();
            collection.Setup(c => c.Append(It.IsAny<ISafeByte>()))
                .Verifiable();
            using var sut = GetSut(collection: collection.Object);
            //Act
            sut.Append(expected);
            //Assert
            collection.Verify(c => c.Append(It.Is<ISafeByte>(b => b.Get() == 55)), Times.Once);
            collection.Verify(c => c.Append(It.Is<ISafeByte>(b => b.Get() == 77)), Times.Once);
        }

        [Test]
        public void AppendByte_ForDisposedObject_throwsObjectDisposedException([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            sut.Dispose();
            //Act
            void AppendingByte() => sut.Append(b);
            //Act
            Assert.That(AppendingByte, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void AppendByte_ForCleanObject_canAppendSingle()
        {
            // Arrange
            const byte expected = 5;
            using var sut = GetSut();
            // Act
            sut.Append(expected);
            // Assert
            var actual = sut.GetByte(0);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void AppendByte_ForCleanObject_canAppendMultiple()
        {
            // Arrange
            byte[] expected = {5,10,15,31,31};
            using var sut = GetSut();
            // Act
            foreach(var @byte in expected)
                sut.Append(@byte);
            // Assert
            var actual = sut.ToByteArray();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void AppendSafeBytes_MultipleBytesOnCleanInstance_appendsAsExpected()
        {
            //Arrange
            byte[] expected = { 3, 5, 10, 20 };
            using var sut = GetSut();
            var toAppend = GetSut();
            foreach (var @byte in expected)
                toAppend.Append(@byte);
            // act
            sut.Append(toAppend);
            // assert
            var actual = sut.ToByteArray();
            Console.WriteLine($"Actual: ${string.Join(",", actual)}{Environment.NewLine}" +
                              $"Expected: ${string.Join(",", expected)}");
            Assert.True(actual.SequenceEqual(expected));
        }

        [Test]
        public void AppendSafeBytes_MultipleBytesOnInstanceWithExistingBytes_appendsAsExpected()
        {
            //Arrange
            byte[] expected = { 3, 5, 10, 20 };
            using var sut = GetSut();
            sut.Append(3);
            sut.Append(5);
            var toAppend = GetSut();
            toAppend.Append(10);
            toAppend.Append(20);
            // act
            sut.Append(toAppend);
            // assert
            var actual = sut.ToByteArray();
            Assert.True(actual.SequenceEqual(expected));
        }

        [Test]
        public void GetByte_OnEmptyInstance_throwsInvalidOperationException()
        {
            //Arrange
            using var sut = GetSut();
            const int position = 0;
            //Act
            void CallingOnEmptyInstance() => sut.GetByte(position);
            //Assert
            Assert.That(CallingOnEmptyInstance, Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void GetByte_ForExistingByteAtStart_retrievesAsExpected()
        {
            //Arrange
            const byte expected = 55;
            using var sut = GetSut();
            sut.Append(expected);
            //Act
            var actual = sut.GetByte(0);
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetByte_ForExistingByteAtEnd_retrievesAsExpected()
        {
            //Arrange
            const byte expected = 55;
            using var sut = GetSut();
            sut.Append(22);
            sut.Append(expected);
            //Act
            var actual = sut.GetByte(1);
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetByte_ForExistingByteInTheMiddle_retrievesAsExpected()
        {
            //Arrange
            const byte expected = 55;
            using var sut = GetSut();
            sut.Append(22);
            sut.Append(expected);
            sut.Append(31);
            //Act
            var actual = sut.GetByte(1);
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ToByteArray_ForEmptyInstance_returnsEmptyArray()
        {
            //Arrange
            using var sut = GetSut();
            //Act
            var actual = sut.ToByteArray();
            //Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Has.Length.Zero);
        }

        [Test]
        public void ToByteArray_ForInstanceWithSingleByte_returnsExpected()
        {
            //Arrange
            byte[] expected = {5};
            using var sut = GetSut();
            sut.Append(5);
            //Act
            var actual = sut.ToByteArray();
            //Assert
            Assert.That(actual.SequenceEqual(expected));
        }

        [Test]
        public void ToByteArray_ForInstanceWithMultipleBytes_returnsExpected()
        {
            //Arrange
            byte[] expected = { 5, 10, 15 };
            using var sut = GetSut();
            foreach(var @byte in expected)
                sut.Append(@byte);
            //Act
            var actual = sut.ToByteArray();
            //Assert
            Assert.That(actual.SequenceEqual(expected));
        }

        [Test]
        public void DeepClone_EmptyInstance_returnsDifferentEmptyInstance()
        {
            // Arrange
            var sut = GetSut();
            // Act
            var clone = sut.DeepClone();
            // Assert
            Assert.False(ReferenceEquals(sut, clone));
            Assert.True(sut.Equals(clone));
            Assert.True(clone.Equals(sut));
        }

        [Test]
        public void DeepClone_Disposed_Throws()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();
            // Act
            void DeepClone() => sut.DeepClone();
            // Assert
            Assert.Throws<ObjectDisposedException>(DeepClone);
        }

        [Test]
        public void DeepClone_WithExistingBytes_returnsDifferentEqualInstance()
        {
            // Arrange
            var sut = GetSut();
            sut.Append(55);
            sut.Append(31);
            // Act
            var clone = sut.DeepClone();
            // Assert
            Assert.False(ReferenceEquals(sut, clone));
            Assert.True(sut.Equals(clone));
            Assert.True(clone.Equals(sut));
        }

        [Test]
        public void Equals_SafeBytes_DifferentLength_returnsFalse()
        {
            // Arrange
            var sut = GetSut();
            sut.Append(1);
            sut.Append(1);
            var other = GetSut();
            other.Append(1);
            // Act
            var equality = sut.Equals(other);
            var equalityOpposite = other.Equals(sut);
            // Assert
            Assert.False(equality);
            Assert.False(equalityOpposite);
        }

        [Test]
        public void Equals_ClearBytes_DifferentLength_returnsFalse()
        {
            // Arrange
            using var sut = GetSut();
            sut.Append(1);
            var other = new byte[] { 1, 1 };
            // Act
            var actual = sut.Equals(other);
            // Assert
            Assert.False(actual);
        }

        [Test]
        public void Equals_SafeBytes_SameLengthDifferentBytes_returnsFalse()
        {
            // Arrange
            var sut = GetSut();
            sut.Append(5);
            sut.Append(10);
            var other = GetSut();
            other.Append(3);
            sut.Append(10);
            // Act
            var equality = sut.Equals(other);
            var equalityOpposite = other.Equals(sut);
            // Assert
            Assert.False(equality);
            Assert.False(equalityOpposite);
        }

        [Test]
        public void Equals_ClearBytes_SameLengthDifferentBytes_returnsFalse()
        {
            // Arrange
            using var sut = GetSut();
            sut.Append(5);
            sut.Append(10);
            var other = new byte[] { 3, 10 };
            // Act
            var actual = sut.Equals(other);
            // Assert
            Assert.False(actual);
        }

        [Test]
        public void Equals_SafeBytes_SameBytes_returnsTrue()
        {
            // Arrange
            var sut = GetSut();
            sut.Append(5);
            sut.Append(10);
            var other = GetSut();
            other.Append(5);
            other.Append(10);
            // Act
            var equality = sut.Equals(other);
            var equalityOpposite = other.Equals(sut);
            // Assert
            Assert.True(equality);
            Assert.True(equalityOpposite);
        }

        [Test]
        public void Equals_ClearBytes_SameBytes_returnsTrue()
        {
            // Arrange
            using var sut = GetSut();
            sut.Append(5);
            sut.Append(10);
            var other = new byte[] { 5, 10 };
            // Act
            var actual = sut.Equals(other);
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
        public void GetHashCode_SameNonEmptyBytes_returnsSame()
        {
            // Arrange
            using var sut = GetSut();
            sut.Append(3);
            sut.Append(5);
            using var other = GetSut();
            other.Append(3);
            other.Append(5);
            // Act
            var hashSut = sut.GetHashCode();
            var hashOther = other.GetHashCode();
            // Assert
            Assert.That(hashSut, Is.EqualTo(hashOther));
        }
        
        [Test]
        public void GetHashCode_DifferentBytesSameLength_returnsFalse()
        {
            // Arrange
            using var sut = GetSut();
            sut.Append(3);
            using var other = GetSut();
            other.Append(5);
            // Act
            var hashSut = sut.GetHashCode();
            var hashOther = other.GetHashCode();
            // Assert
            Assert.That(hashSut, Is.Not.EqualTo(hashOther));
        }
        
        [Test]
        public void GetHashCode_DifferentBytesDifferentLength_returnsFalse()
        {
            // Arrange
            using var sut = GetSut();
            sut.Append(1);
            using var other = GetSut();
            other.Append(1);
            other.Append(1);
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
            mock.Setup(m=>m.Dispose()).Verifiable();
            var sut = GetSut(collection: mock.Object);
            // Act
            sut.Dispose();
            // Assert
            mock.Verify(m=>m.Dispose(), Times.Once);
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
        public void IsDisposed_NonEmptyInstance_returnsFalse()
        {
            // Arrange & act
            using var sut = GetSut();
            sut.Append(5);
            sut.Append(10);
            // Assert
            Assert.False(sut.IsDisposed);
        }

        protected override ISafeBytes GetSut() => GetSut();
        private static ISafeBytes GetSut(ISafeByteCollection collection = null, ISafeByteFactory factory = null)
        {
            return new SafeBytes(
                Stubs.Get<IFastRandom>(),
                factory ?? Stubs.Get<ISafeByteFactory>(),
                Stubs.GetFactory<ISafeBytes>(),
                Stubs.GetFactory(collection));
        }
    }
}