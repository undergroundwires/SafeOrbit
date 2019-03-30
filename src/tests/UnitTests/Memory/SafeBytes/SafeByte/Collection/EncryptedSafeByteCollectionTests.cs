using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Cryptography.Encryption;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Memory.SafeBytesServices.Factory;
using SafeOrbit.Tests;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Memory.SafeBytesServices.Collection
{
    [TestFixture]
    internal class EncryptedSafeByteCollectionTests : TestsFor<ISafeByteCollection>
    {
        private static ISafeByte GetSafeByteFor(byte b)
        {
            var safeByteFactory = Stubs.Get<ISafeByteFactory>();
            return safeByteFactory.GetByByte(b);
        }

        protected override ISafeByteCollection GetSut()
        {
            return new EncryptedSafeByteCollection(
                encryptor: Stubs.Get<IFastEncryptor>(),
                memoryProtector: Stubs.Get<IByteArrayProtector>(),
                fastRandom: Stubs.Get<IFastRandom>(),
                safeByteFactory: Stubs.Get<ISafeByteFactory>(),
                serializer: Stubs.Get<IByteIdListSerializer<int>>());
        }

        //** Append **//
        [Test]
        public void Append_ForDisposedObject_throwsObjectDisposedException([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            sut.Dispose();
            //Act
            void AppendByte() => sut.Append(GetSafeByteFor(b));
            //Act
            Assert.That(AppendByte, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public async Task Append_GetAsync_SafeBytes_AppendsSingle_CanGet()
        {
            //Arrange
            var sut = GetSut();
            const byte expected = 55;
            sut.Append(GetSafeByteFor(expected));
            //Act
            var actual = await sut.GetAsync(0);
            //Assert
            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public async Task Append_GetAsync_SafeBytes_AppendsAtTheEnd_CanGet([Random(0, 256, 1)] byte b1,
            [Random(0, 256, 1)] byte b2, [Random(0, 256, 1)] byte b3)
        {
            //Arrange
            var sut = GetSut();
            sut.Append(GetSafeByteFor(b1));
            sut.Append(GetSafeByteFor(b2));
            sut.Append(GetSafeByteFor(b3));
            //Act
            var b2Back = await sut.GetAsync(1);
            var b3Back = await sut.GetAsync(2);
            //Assert
            Assert.That(b2, Is.EqualTo(b2Back));
            Assert.That(b3, Is.EqualTo(b3Back));
        }

        [Test]
        public void Append_WhenArgumentIsNull_throwsArgumentNullException()
        {
            //Arrange
            var sut = GetSut();
            ISafeByte nullInstance = null;
            //Act
            void AppendNull() => sut.Append(nullInstance);
            //Act
            Assert.That(AppendNull, Throws.TypeOf<ArgumentNullException>());
        }

        //** Dispose **//
        [Test]
        public void Dispose_OnAnInstanceWithBytes_SetsLengthToZero([Random(0, 256, 1)] byte b1,
            [Random(0, 256, 1)] byte b2, [Random(0, 256, 1)] byte b3)
        {
            //arrange
            var sut = GetSut();
            sut.Append(GetSafeByteFor(b1));
            sut.Append(GetSafeByteFor(b2));
            sut.Append(GetSafeByteFor(b3));
            //act
            sut.Dispose();
            var actual = sut.Length;
            //assert
            Assert.That(actual, Is.Zero);
        }

        //** Get **//
        [Test]
        public void GetAsync_OnDisposedObject_throwsObjectDisposedException([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            sut.Append(GetSafeByteFor(b));
            var position = 0;
            //Act
            sut.Dispose();
            async Task CallOnDisposedObject() => await sut.GetAsync(position);
            //Assert
            Assert.ThrowsAsync<ObjectDisposedException>(CallOnDisposedObject);
        }

        [Test]
        public void GetAsync_OnEmptyInstance_throwsInvalidOperationException()
        {
            //Arrange
            var sut = GetSut();
            var position = 0;
            //Act
            async Task CallOnEmptyInstance() => await sut.GetAsync(position);
            //Assert
            Assert.ThrowsAsync<InvalidOperationException>(CallOnEmptyInstance);
        }

        [Test]
        public void GetAsync_WhenRequestedPositionIsLowerThanRange_throwsArgumentOutOfRangeException(
            [Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            sut.Append(GetSafeByteFor(b));
            var position = -1;
            //Act
            async Task CallWithBadParameter() => await sut.GetAsync(position);
            //Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(CallWithBadParameter);
        }

        [Test]
        public async Task GetAsync_ForMultipleAppendedBytes_returnsTheBytes([Random(0, 256, 1)] byte b1,
            [Random(0, 256, 1)] byte b2, [Random(0, 256, 1)] byte b3)
        {
            //Arrange
            var sut = GetSut();
            sut.Append(GetSafeByteFor(b1));
            sut.Append(GetSafeByteFor(b2));
            sut.Append(GetSafeByteFor(b3));
            //Act
            var b1Back = await sut.GetAsync(0);
            var b2Back = await sut.GetAsync(1);
            var b3Back = await sut.GetAsync(2);
            //Assert
            Assert.That(b1Back, Is.EqualTo(b1));
            Assert.That(b2Back, Is.EqualTo(b2));
            Assert.That(b3Back, Is.EqualTo(b3));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public async Task GetByte_ForSingleAppendedByte_ReturnsByte(byte b)
        {
            //Arrange
            var sut = GetSut();
            var safeByte = GetSafeByteFor(b);
            //Act
            sut.Append(safeByte);
            var safebyteBack = await sut.GetAsync(0);
            //Assert
            Assert.That(safeByte, Is.EqualTo(safebyteBack));
        }

        [Test]
        public void GetByte_WhenRequestedPositionIsHigherThanRange_throwsArgumentOutOfRangeException(
            [Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            sut.Append(GetSafeByteFor(b));
            const int position = 1;
            //Act
            async Task CallWithBadParameter() => await sut.GetAsync(position);
            //Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(CallWithBadParameter);
        }

        [Test]
        public void Length_AfterSingleAppend_Increases([Random(0, 256, 1)] byte b)
        {
            //arrange
            var sut = GetSut();
            const int expected = 1;
            sut.Append(GetSafeByteFor(b));
            //act
            var actual = sut.Length;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        //** Length **//
        [Test]
        public void Length_ForAFreshInstance_IsZero()
        {
            //arrange
            var sut = GetSut();
            const int expected = 0;
            //act
            var actual = sut.Length;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Length_IncreasesAfterEachAppend_Increases([Random(0, 256, 1)] byte b1,
            [Random(0, 256, 1)] byte b2, [Random(0, 256, 1)] byte b3, [Random(0, 256, 1)] byte b4)
        {
            //arrange
            var sut = GetSut();
            const int expected1 = 0;
            const int expected2 = 1;
            const int expected3 = 2;
            const int expected4 = 3;
            //act
            var actual1 = sut.Length;
            sut.Append(GetSafeByteFor(b1));
            var actual2 = sut.Length;
            sut.Append(GetSafeByteFor(b2));
            var actual3 = sut.Length;
            sut.Append(GetSafeByteFor(b3));
            var actual4 = sut.Length;
            //assert
            Assert.That(actual1, Is.EqualTo(expected1));
            Assert.That(actual2, Is.EqualTo(expected2));
            Assert.That(actual3, Is.EqualTo(expected3));
            Assert.That(actual4, Is.EqualTo(expected4));
        }

        [Test]
        public void ToByteArray_ForInstanceHoldingMultipleBytes_returnsTheArrayOfBytes([Random(0, 256, 1)] byte b1,
            [Random(0, 256, 1)] byte b2, [Random(0, 256, 1)] byte b3)
        {
            //Arrange
            var bytes = new[] {b1, b2, b3};
            var sut = GetSut();
            sut.Append(GetSafeByteFor(b1));
            sut.Append(GetSafeByteFor(b2));
            sut.Append(GetSafeByteFor(b3));
            //Act
            var byteArray = sut.ToDecryptedBytes();
            //Assert
            CollectionAssert.AreEqual(byteArray, bytes);
        }

        [Test]
        public void ToByteArray_ForInstanceHoldingSingleByte_returnsTheArrayOfBytes([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var bytes = new[] {b};
            var sut = GetSut();
            sut.Append(GetSafeByteFor(b));
            //Act
            var byteArray = sut.ToDecryptedBytes();
            //Assert
            CollectionAssert.AreEqual(byteArray, bytes);
        }

        [Test]
        public void ToDecryptedBytes_OnDisposedObject_throwsObjectDisposedException([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            sut.Append(GetSafeByteFor(b));
            const int position = 0;
            //Act
            sut.Dispose();
            async Task CallingOnDisposedObject() => await sut.GetAsync(position);
            //Assert
            Assert.ThrowsAsync<ObjectDisposedException>(CallingOnDisposedObject);
        }

        //** ToDecryptedBytes **//
        [Test]
        public void ToDecryptedBytes_OnEmptyInstance_throwsInvalidOperationException()
        {
            //Arrange
            var sut = GetSut();
            //Act
            void CallingOnEmptyInstance() => sut.ToDecryptedBytes();
            //Assert
            Assert.That(CallingOnEmptyInstance, Throws.TypeOf<InvalidOperationException>());
        }
    }
}