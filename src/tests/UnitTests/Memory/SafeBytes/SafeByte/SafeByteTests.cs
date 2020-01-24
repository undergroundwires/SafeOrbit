using System;
using System.Threading.Tasks;
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
        public async Task Id_Disposed_Throws()
        {
            // Arrange
            var sut = GetSut();
            await sut.SetAsync(5);
            sut.Dispose();

            // Act
            void GetId() => _ = sut.Id;

            // Assert
            Assert.Throws<ObjectDisposedException>(GetId);
        }

        [Test]
        public void Id_ByteIsNotSet_Throws()
        {
            // Arrange
            using var sut = GetSut();

            // Act
            void GetId() => _ = sut.Id;

            // Assert
            Assert.Throws<InvalidOperationException>(GetId);
        }

        [Test]
        public async Task DeepClone_ClonedInstanceWithSameValue_AreNotReferToSameObject([Random(0, 256, 1)] byte b)
        {
            // Arrange
            var sut = GetSut();
            await sut.SetAsync(b);

            // Act
            var other = sut.DeepClone();
            // Assert

            Assert.False(ReferenceEquals(sut, other));
        }

        [Test]
        public async Task DeepClone_ClonedObjectsGet_returnsEqualByte([Random(0, 256, 1)] byte b)
        {
            // Arrange
            using var sut = GetSut();
            await sut.SetAsync(b);
            var cloned = sut.DeepClone();

            // Act
            var byteBack = await sut.GetAsync();
            var byteBackFromClone = await cloned.GetAsync();

            // Assert
            Assert.That(byteBack, Is.EqualTo(byteBackFromClone));
        }

        [Test]
        public async Task DeepClone_ClonedObjectsValue_isEqual([Random(0, 256, 1)] byte b)
        {
            // Arrange
            var sut = GetSut();
            await sut.SetAsync(b);

            // Act
            var isEqual = sut.DeepClone().Equals(sut);

            // Assert
            Assert.That(isEqual, Is.True);
        }

        [Test]
        public void DeepClone_ForNotSetByte_throwsInvalidOperationException()
        {
            // Arrange
            using var sut = GetSut();

            // Act
            void CallDeepClone() => sut.DeepClone();

            // Assert
            Assert.That(CallDeepClone, Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public async Task DeepClone_Disposed_Throws()
        {
            // Arrange
            var sut = GetSut();
            await sut.SetAsync(5);
            sut.Dispose();

            // Act
            void DeepClone() => sut.DeepClone();

            // Assert
            Assert.Throws<ObjectDisposedException>(DeepClone);
        }


        [Test]
        public async Task DeepClone_EncryptedByte_IsCloned()
        {
            // Arrange
            var mock = new Mock<IMemoryProtectedBytes>();
            mock.Setup(m => m.BlockSizeInBytes).Returns(5);
            mock.Setup(m => m.DeepClone()).Returns(() => mock.Object).Verifiable();
            using var sut = GetSut(encryptedByte: mock.Object);
            await sut.SetAsync(5);

            // Act
            sut.DeepClone();

            // Assert
            mock.Verify(m => m.DeepClone(), Times.Once);
        }

        [Test]
        public async Task DeepClone_EncryptionKey_IsCloned()
        {
            // Arrange
            var mock = new Mock<IMemoryProtectedBytes>();
            mock.Setup(m => m.BlockSizeInBytes).Returns(5);
            mock.Setup(m => m.DeepClone()).Returns(() => mock.Object).Verifiable();
            using var sut = GetSut(encryptionKey: mock.Object);
            await sut.SetAsync(5);

            // Act
            sut.DeepClone();

            // Assert
            mock.Verify(m => m.DeepClone(), Times.Once);
        }

        [Test]
        public async Task EqualsSafeByte_SelfIsDisposed_Throws()
        {
            // Arrange
            var sut = GetSut();
            await sut.SetAsync(5);
            sut.Dispose();

            // Act
            void Equals() => sut.Equals(GetSut());

            // Assert
            Assert.Throws<ObjectDisposedException>(Equals);
        }

        [Test]
        public async Task EqualsSafeByte_OtherIsDisposed_Throws()
        {
            // Arrange
            var sut = GetSut();
            await sut.SetAsync(5);
            var other = GetSut();
            await other.SetAsync(5);
            other.Dispose();

            // Act
            void Equals() => sut.Equals(other);
            
            // Assert
            Assert.Throws<ObjectDisposedException>(Equals);
        }

        [Test]
        public void EqualsSafeByte_ForTwoEmptyInstances_Throws()
        {
            // Arrange
            var sut = GetSut();
            var sut2 = GetSut();

            // Act
            void Equals() => sut.Equals(sut2);
            void EqualsOtherWay() => sut2.Equals(sut);

            // Assert
            Assert.Throws<InvalidOperationException>(Equals);
            Assert.Throws<InvalidOperationException>(EqualsOtherWay);
        }

        [Test]
        public async Task EqualsSafeByte_SingleEmptyInstance_Throws()
        {
            // Arrange
            using var sut = GetSut();
            using var obj2 = GetSut();
            await obj2.SetAsync(5);

            // Act
            void Equals() => sut.Equals(obj2);

            // Assert
            Assert.Throws<InvalidOperationException>(Equals);
        }

        [Test]
        public void EqualsObject_WhenParameterIsNullObject_returnsFalse()
        {
            // Arrange
            using var sut = GetSut();
            object nullInstance = null;

            // Act
            var equals = sut.Equals(nullInstance);

            // Assert
            Assert.That(equals, Is.False);
        }

        [Test]
        public void EqualsObject_WhenParameterIsUnknownType_throws()
        {
            // Arrange
            using var sut = GetSut();
            var unknownType = new SafeByteTests();

            // Act
            void Equals() => sut.Equals(unknownType);

            // Assert
            Assert.Throws<ArgumentException>(Equals);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentBytePairs))]
        public async Task EqualsSafeByte_ForDifferentInstancesHoldingDifferentBytes_returnsFalse(byte b1, byte b2)
        {
            // Arrange
            using var sut = GetSut();
            var holdingDifferent = GetSut();
            await sut.SetAsync(b1);
            await holdingDifferent.SetAsync(b2);

            // Act
            var equals = sut.Equals(holdingDifferent);
            var equalsOpposite = holdingDifferent.Equals(sut);

            // Assert
            Assert.That(equals, Is.False);
            Assert.That(equalsOpposite, Is.False);
        }

        [Test]
        public async Task EqualsSafeByte_ForDifferentInstancesHoldingSameByte_returnsTrue([Random(0, 256, 1)] byte b)
        {
            // Arrange
            var sut = GetSut();
            var holdingSame = GetSut();
            await sut.SetAsync(b);
            await holdingSame.SetAsync(b);

            // Act
            var equals = sut.Equals(holdingSame);
            var equalsOpposite = holdingSame.Equals(sut);

            // Assert
            Assert.That(equals, Is.True);
            Assert.That(equalsOpposite, Is.True);
        }

        [Test]
        public async Task EqualsSafeByte_ForSameInstances_returnsTrue([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            await sut.SetAsync(b);
            var sameObject = sut;

            // Act
            var equals = sut.Equals(sameObject);
            var equalsOpposite = sameObject.Equals(sut);

            // Assert
            Assert.That(equals, Is.True);
            Assert.That(equalsOpposite, Is.True);
        }

        [Test]
        public void EqualsSafeByte_ParameterIsNullSafeByte_returnsFalse()
        {
            // Arrange
            using var sut = GetSut();
            ISafeByte nullInstance = null;

            // Act
            var equals = sut.Equals(nullInstance);

            // Assert
            Assert.That(equals, Is.False);
        }

        [Test]
        public void EqualsAsyncByte_NotInitialized_ThrowsException()
        {
            // Arrange
            using var sut = GetSut();

            // Act
            Task EqualsAsync() => sut.EqualsAsync(5);

            // Assert
            Assert.ThrowsAsync<InvalidOperationException>(EqualsAsync);
        }

        [Test]
        public void EqualsAsyncByte_Disposed_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();

            // Act
            Task EqualsAsync() => sut.EqualsAsync(5);

            // Assert
            Assert.ThrowsAsync<ObjectDisposedException>(EqualsAsync);
        }


        [Test]
        public void EqualsAsyncByte_OnEmptyInstance_Throws()
        {
            // Arrange
            using var sut = GetSut();

            // Act
            Task EqualsAsync() => sut.EqualsAsync(5);

            // Assert
            Assert.ThrowsAsync<InvalidOperationException>(EqualsAsync);
        }

        [Test]
        public async Task EqualsAsyncByte_SameByte_ReturnsTrue()
        {
            // Arrange
            const byte @byte = 31;
            var sut = GetSut();
            await sut.SetAsync(@byte);

            // Act
            var actual = await sut.EqualsAsync(@byte);

            // Assert
            Assert.True(actual);
        }

        [Test]
        public async Task EqualsAsyncByte_DifferentByte_ReturnsFalse()
        {
            // Arrange
            const byte @byte = 31, differentByte = 52;
            var sut = GetSut();
            await sut.SetAsync(@byte);

            // Act
            var actual = await sut.EqualsAsync(differentByte);

            // Assert
            Assert.False(actual);
        }

        //** Get() **//
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public async Task GetAsync_ReturnsThePreviouslySetByte_returnsTrue(byte expected)
        {
            // Arrange
            using var sut = GetSut();

            // Act
            await sut.SetAsync(expected);
            var actual = await sut.GetAsync();

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetAsync_WhenCalledForNotSetObject_throwsInvalidOperationException()
        {
            // Arrange
            using var sut = GetSut();

            // Act
            Task CallGet() => sut.GetAsync();

            // Assert
            Assert.ThrowsAsync<InvalidOperationException>(CallGet);
        }


        [Test]
        public async Task GetAsync_ObjectIsDisposed_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            await sut.SetAsync(5);
            sut.Dispose();

            // Act
            Task CallGet() => sut.GetAsync();

            // Assert
            Assert.ThrowsAsync<ObjectDisposedException>(CallGet);
        }

        //** GetHashCode() **//
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public async Task GetHashCode_ForDistinctObjectsHavingSameValue_returnsTrue(byte b)
        {
            // Arrange
            using var sut = GetSut();
            using var holdingSame = GetSut();
            await sut.SetAsync(b);
            await holdingSame.SetAsync(b);

            // Act
            var hashCode = sut.GetHashCode();
            var sameHashCode = holdingSame.GetHashCode();

            // Assert
            Assert.AreEqual(hashCode, sameHashCode);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentBytePairs))]
        public async Task GetHashCode_ForTwoNonEqualObjects_returnsFalse(byte b1, byte b2)
        {
            // Arrange
            using var sut = GetSut();
            using var holdingDifferent = GetSut();
            await sut.SetAsync(b1);
            await holdingDifferent.SetAsync(b2);

            // Act
            var hashCode = sut.GetHashCode();
            var hashCodeForOtherByte = holdingDifferent.GetHashCode();

            // Assert
            Assert.That(hashCode, Is.Not.EqualTo(hashCodeForOtherByte));
        }

        [Test]
        public void Id_GettingWithoutSettingAnyByte_throws()
        {
            // Arrange
            using var sut = GetSut();
            int temp;

            // Act
            void GetIdWithoutSettingByte() => temp = sut.Id;

            // Act
            Assert.That(GetIdWithoutSettingByte, Throws.TypeOf<InvalidOperationException>());
        }

        //** IsByteSet **//
        [Test]
        public void IsByteSet_BeforeInvokingSetMethod_returnsFalse()
        {
            // Arrange
            using var sut = GetSut();

            // Act
            var isByteSet = sut.IsByteSet;

            // Assert
            Assert.That(isByteSet, Is.False);
        }

        //** Set() **//
        [Test]
        public async Task SetAsync_SetsIsByteSetToTrue_returnsTrue()
        {
            // Arrange
            using var sut = GetSut();

            // Act
            await sut.SetAsync(30);
            var isByteSet = sut.IsByteSet;

            // Assert
            Assert.That(isByteSet, Is.True);
        }

        [Test]
        public async Task SetAsync_WhenCalledTwice_throwsInvalidOperationException(
            [Random(0, 256, 1)] byte b1,
            [Random(0, 256, 1)] byte b2)
        {
            // Arrange
            using var sut = GetSut();
            
            // Act
            await sut.SetAsync(b1);
            Task CallOneMoreTime() => sut.SetAsync(b2);

            // Assert
            Assert.ThrowsAsync<InvalidOperationException>(CallOneMoreTime);
        }

        [Test]
        public void Set_Disposed_ThrowsException()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();

            // Act
            Task Set() => sut.SetAsync(30);

            // Assert
            Assert.ThrowsAsync<ObjectDisposedException>(Set);
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