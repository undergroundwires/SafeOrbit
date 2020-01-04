using System;
using Moq;
using NUnit.Framework;
using SafeOrbit.Cryptography.Encryption;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Memory.SafeBytesServices.Id;
using SafeOrbit.Tests;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Memory.SafeBytesServices
{
    /// <seealso cref="ISafeByte" />
    /// <seealso cref="SafeByte" />
    [TestFixture]
    internal class SafeByteTests : TestsFor<ISafeByte>
    {
        [Test]
        public void Id_Disposed_Throws()
        {
            // Arrange
            var sut = GetSut();
            sut.Set(5);
            sut.Dispose();

            // Act
            void GetId()
            {
                var temp = sut.Id;
            }

            // Assert
            Assert.Throws<ObjectDisposedException>(GetId);
        }

        [Test]
        public void Id_ByteIsNotSet_Throws()
        {
            // Arrange
            using var sut = GetSut();

            // Act
            void GetId()
            {
                var temp = sut.Id;
            }

            // Assert
            Assert.Throws<InvalidOperationException>(GetId);
        }

        [Test]
        public void DeepClone_ClonedInstanceWithSameValue_AreNotReferToSameObject([Random(0, 256, 1)] byte b)
        {
            // Arrange
            var sut = GetSut();
            sut.Set(b);
            // Act
            var other = sut.DeepClone();
            // Assert
            Assert.False(ReferenceEquals(sut, other));
        }

        [Test]
        public void DeepClone_ClonedObjectsGet_returnsEqualByte([Random(0, 256, 1)] byte b)
        {
            //Arrange
            using var sut = GetSut();
            sut.Set(b);
            var cloned = sut.DeepClone();
            //Act
            var byteBack = sut.Get();
            var byteBackFromClone = cloned.Get();
            //Assert
            Assert.That(byteBack, Is.EqualTo(byteBackFromClone));
        }

        [Test]
        public void DeepClone_ClonedObjectsValue_isEqual([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            sut.Set(b);
            //Act
            var isEqual = sut.DeepClone().Equals(sut);
            //Assert
            Assert.That(isEqual, Is.True);
        }

        [Test]
        public void DeepClone_ForNotSetByte_throwsInvalidOperationException()
        {
            //Arrange
            using var sut = GetSut();

            //Act
            void CallDeepClone() => sut.DeepClone();
            //Assert
            Assert.That(CallDeepClone, Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void DeepClone_Disposed_Throws()
        {
            // Arrange
            var sut = GetSut();
            sut.Set(5);
            sut.Dispose();

            // Act
            void DeepClone() => sut.DeepClone();
            // Assert
            Assert.Throws<ObjectDisposedException>(DeepClone);
        }


        [Test]
        public void DeepClone_EncryptedByte_IsCloned()
        {
            //Arrange
            var mock = new Mock<IMemoryProtectedBytes>();
            mock.Setup(m => m.BlockSizeInBytes).Returns(5);
            mock.Setup(m => m.DeepClone()).Returns(() => mock.Object).Verifiable();
            using var sut = GetSut(encryptedByte: mock.Object);
            sut.Set(5);
            //Act
            sut.DeepClone();
            //Assert
            mock.Verify(m => m.DeepClone(), Times.Once);
        }

        [Test]
        public void DeepClone_EncryptionKey_IsCloned()
        {
            //Arrange
            var mock = new Mock<IMemoryProtectedBytes>();
            mock.Setup(m => m.BlockSizeInBytes).Returns(5);
            mock.Setup(m => m.DeepClone()).Returns(() => mock.Object).Verifiable();
            using var sut = GetSut(encryptionKey: mock.Object);
            sut.Set(5);
            //Act
            sut.DeepClone();
            //Assert
            mock.Verify(m => m.DeepClone(), Times.Once);
        }

        [Test]
        public void Equals_ForObjectWithValueAndNoValue_returnsFalse([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            sut.Set(b);
            var sut2 = GetSut();
            //Act
            var equals = sut.Equals(sut2);
            var equals2 = sut2.Equals(sut);
            //Assert
            Assert.That(equals, Is.False);
            Assert.That(equals2, Is.False);
        }

        [Test]
        public void Equals_SelfIsDisposed_Throws()
        {
            //Arrange
            var sut = GetSut();
            sut.Set(5);
            sut.Dispose();

            //Act
            void Equals() => sut.Equals(GetSut());
            //Assert
            Assert.Throws<ObjectDisposedException>(Equals);
        }

        [Test]
        public void Equals_OtherIsDisposed_Throws()
        {
            //Arrange
            var sut = GetSut();
            sut.Set(5);
            var other = GetSut();
            other.Set(5);
            other.Dispose();

            //Act
            void Equals() => sut.Equals(other);
            //Assert
            Assert.Throws<ObjectDisposedException>(Equals);
        }

        [Test]
        public void Equals_ForTwoEmptyInstances_ReturnsTrue()
        {
            //Arrange
            var sut = GetSut();
            var sut2 = GetSut();
            //Act
            var equals = sut.Equals(sut2);
            var equals2 = sut2.Equals(sut);
            //Assert
            Assert.That(equals, Is.True);
            Assert.That(equals2, Is.True);
        }

        [Test]
        public void Equals_ObjectWithoutByteWithByte_Throws()
        {
            //Arrange
            using var sut = GetSut();

            //Act
            void Equals() => sut.Equals(5);
            //Assert
            Assert.Throws<InvalidOperationException>(Equals);
        }

        [Test]
        public void EqualsObject_WhenParameterIsNullObject_returnsFalse()
        {
            //Arrange
            using var sut = GetSut();
            object nullInstance = null;
            //Act
            var equals = sut.Equals(nullInstance);
            //Assert
            Assert.That(equals, Is.False);
        }

        [Test]
        public void EqualsObject_WhenParameterIsUnknownType_throws()
        {
            //Arrange
            using var sut = GetSut();
            var unknownType = new SafeByteTests();

            //Act
            void Equals() => sut.Equals(unknownType);
            //Assert
            Assert.Throws<ArgumentException>(Equals);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentBytePairs))]
        public void EqualsByte_ForDifferentBytes_returnsFalse(byte b1, byte b2)
        {
            //Arrange
            using var sut = GetSut();
            sut.Set(b1);
            //Act
            var equals = sut.Equals(b2);
            //Assert
            Assert.That(equals, Is.False);
        }

        [Test]
        public void EqualsByte_ForSameByte_returnsTrue([Random(0, 256, 1)] byte b)
        {
            //Arrange
            using var sut = GetSut();
            sut.Set(b);
            //Act
            var isEqual = sut.Equals(b);
            //Assert
            Assert.That(isEqual, Is.True);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentBytePairs))]
        public void EqualsSafeByte_ForDifferentInstancesHoldingDifferentBytes_returnsFalse(byte b1, byte b2)
        {
            //Arrange
            using var sut = GetSut();
            var holdingDifferent = GetSut();
            sut.Set(b1);
            holdingDifferent.Set(b2);
            //Act
            var equals = sut.Equals(holdingDifferent);
            var equalsOpposite = holdingDifferent.Equals(sut);
            //Assert
            Assert.That(equals, Is.False);
            Assert.That(equalsOpposite, Is.False);
        }

        [Test]
        public void EqualsSafeByte_ForDifferentInstancesHoldingSameByte_returnsTrue([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            var holdingSame = GetSut();
            sut.Set(b);
            holdingSame.Set(b);
            //Act
            var equals = sut.Equals(holdingSame);
            var equalsOpposite = holdingSame.Equals(sut);
            //Assert
            Assert.That(equals, Is.True);
            Assert.That(equalsOpposite, Is.True);
        }

        [Test]
        public void EqualsSafeByte_ForSameInstances_returnsTrue([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            sut.Set(b);
            var sameObject = sut;
            //Act
            var equals = sut.Equals(sameObject);
            var equalsOpposite = sameObject.Equals(sut);
            //Assert
            Assert.That(equals, Is.True);
            Assert.That(equalsOpposite, Is.True);
        }

        [Test]
        public void EqualsSafeByte_ParameterIsNullSafeByte_returnsFalse()
        {
            //Arrange
            using var sut = GetSut();
            ISafeByte nullInstance = null;
            //Act
            var equals = sut.Equals(nullInstance);
            //Assert
            Assert.That(equals, Is.False);
        }

        [Test]
        public void EqualsByte_NotInitialized_ThrowsException()
        {
            //Arrange
            using var sut = GetSut();

            //Act
            void Equals() => sut.Equals(5);
            //Assert
            Assert.Throws<InvalidOperationException>(Equals);
        }

        [Test]
        public void EqualsByte_Disposed_ThrowsException()
        {
            //Arrange
            var sut = GetSut();
            sut.Dispose();

            //Act
            void Equals() => sut.Equals(5);
            //Assert
            Assert.Throws<ObjectDisposedException>(Equals);
        }

        //** Get() **//
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public void Get_ReturnsThePreviouslySetByte_returnsTrue(byte expected)
        {
            //Arrange
            using var sut = GetSut();
            //Act
            sut.Set(expected);
            var actual = sut.Get();
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Get_WhenCalledForNotSetObject_throwsInvalidOperationException()
        {
            //Arrange
            using var sut = GetSut();

            //Act
            void CallGet() => sut.Get();
            //Assert
            Assert.That(CallGet, Throws.TypeOf<InvalidOperationException>());
        }


        [Test]
        public void Get_ObjectIsDisposed_ThrowsException()
        {
            //Arrange
            var sut = GetSut();
            sut.Set(5);
            sut.Dispose();

            //Act
            void CallGet() => sut.Get();
            //Assert
            Assert.That(CallGet, Throws.TypeOf<ObjectDisposedException>());
        }

        //** GetHashCode() **//
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public void GetHashCode_ForDistinctObjectsHavingSameValue_returnsTrue(byte b)
        {
            //Arrange
            using var sut = GetSut();
            using var holdingSame = GetSut();
            sut.Set(b);
            holdingSame.Set(b);
            //Act
            var hashCode = sut.GetHashCode();
            var sameHashCode = holdingSame.GetHashCode();
            //Assert
            Assert.AreEqual(hashCode, sameHashCode);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentBytePairs))]
        public void GetHashCode_ForTwoNonEqualObjects_returnsFalse(byte b1, byte b2)
        {
            //Arrange
            using var sut = GetSut();
            using var holdingDifferent = GetSut();
            sut.Set(b1);
            holdingDifferent.Set(b2);
            //Act
            var hashCode = sut.GetHashCode();
            var hashCodeForOtherByte = holdingDifferent.GetHashCode();
            //Assert
            Assert.That(hashCode, Is.Not.EqualTo(hashCodeForOtherByte));
        }

        [Test]
        public void Id_GettingWithoutSettingAnyByte_throws()
        {
            //Arrange
            using var sut = GetSut();
            int temp;

            //Act
            void GetIdWithoutSettingByte() => temp = sut.Id;
            //Act
            Assert.That(GetIdWithoutSettingByte, Throws.TypeOf<InvalidOperationException>());
        }

        //** IsByteSet **//
        [Test]
        public void IsByteSet_BeforeInvokingSetMethod_returnsFalse()
        {
            //Arrange
            using var sut = GetSut();
            //Act
            var isByteSet = sut.IsByteSet;
            //Assert
            Assert.That(isByteSet, Is.False);
        }

        //** Set() **//
        [Test]
        public void Set_SetsIsByteSetToTrue_returnsTrue()
        {
            //Arrange
            using var sut = GetSut();
            //Act
            sut.Set(30);
            var isByteSet = sut.IsByteSet;
            //Assert
            Assert.That(isByteSet, Is.True);
        }

        [Test]
        public void Set_WhenCalledTwice_throwsInvalidOperationException(
            [Random(0, 256, 1)] byte b1,
            [Random(0, 256, 1)] byte b2)
        {
            //Arrange
            using var sut = GetSut();
            //Act
            sut.Set(b1);

            void CallOneMoreTime() => sut.Set(b2);
            //Assert
            Assert.That(CallOneMoreTime, Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Set_Disposed_ThrowsException()
        {
            //Arrange
            var sut = GetSut();
            sut.Dispose();

            //Act
            void Set() => sut.Set(30);
            //Assert
            Assert.Throws<ObjectDisposedException>(Set);
        }

        [Test]
        public void Dispose_DisposesInnerByte()
        {
            // Arrange
            var mock = new Mock<IMemoryProtectedBytes>();
            mock.Setup(m => m.BlockSizeInBytes).Returns(5);
            mock.Setup(m => m.Dispose()).Verifiable();
            var sut = GetSut(encryptedByte: mock.Object);
            // Act
            sut.Dispose();
            // Assert
            mock.Verify(m => m.Dispose(), Times.Once);
        }

        [Test]
        public void Dispose_DisposesInnerKey()
        {
            // Arrange
            var mock = new Mock<IMemoryProtectedBytes>();
            mock.Setup(m => m.BlockSizeInBytes).Returns(5);
            mock.Setup(m => m.Dispose()).Verifiable();
            var sut = GetSut(encryptionKey: mock.Object);
            // Act
            sut.Dispose();
            // Assert
            mock.Verify(m => m.Dispose(), Times.Once);
        }

        [Test]
        public void Dispose_CalledTwice_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();

            // Act
            void Dispose() => sut.Dispose();
            // Assert
            Assert.Throws<ObjectDisposedException>(Dispose);
        }


        private static ISafeByte GetSut(IMemoryProtectedBytes encryptedByte = null,
            IMemoryProtectedBytes encryptionKey = null)
        {
            return new SafeByte
            (
                Stubs.Get<IFastEncryptor>(),
                Stubs.Get<IFastRandom>(),
                Stubs.Get<IByteIdGenerator>(),
                encryptedByte ?? Stubs.Get<IMemoryProtectedBytes>(),
                encryptionKey ?? Stubs.Get<IMemoryProtectedBytes>()
            );
        }

        protected override ISafeByte GetSut() => GetSut();
    }
}