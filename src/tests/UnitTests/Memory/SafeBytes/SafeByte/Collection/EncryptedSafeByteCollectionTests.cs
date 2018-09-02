
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
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
        private ISafeByte GetSafeByteFor(byte b)
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
            TestDelegate appendingByte = () => sut.Append(GetSafeByteFor(b));
            //Act
            Assert.That(appendingByte, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void Append_Get_SafeBytes_AppendsAtTheEnd_returnsTrue([Random(0, 256, 1)] byte b1,
            [Random(0, 256, 1)] byte b2, [Random(0, 256, 1)] byte b3)
        {
            //Arrange
            var sut = GetSut();
            sut.Append(GetSafeByteFor(b1));
            sut.Append(GetSafeByteFor(b2));
            sut.Append(GetSafeByteFor(b3));
            //Act
            var b2Back = sut.Get(1);
            var b3Back = sut.Get(2);
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
            TestDelegate appendingNull = () => sut.Append(nullInstance);
            //Act
            Assert.That(appendingNull, Throws.TypeOf<ArgumentNullException>());
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
        public void Get_OnDisposedObject_throwsObjectDisposedException([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            sut.Append(GetSafeByteFor(b));
            var position = 0;
            //Act
            sut.Dispose();
            TestDelegate callingOnDisposedObject = () => sut.Get(position);
            //Assert
            Assert.That(callingOnDisposedObject, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void Get_OnEmptyInstance_throwsInvalidOperationException()
        {
            //Arrange
            var sut = GetSut();
            var position = 0;
            //Act
            TestDelegate callingOnEmptyInstance = () => sut.Get(position);
            //Assert
            Assert.That(callingOnEmptyInstance, Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Get_WhenRequestedPositionIsLowerThanRange_throwsArgumentOutOfRangeException(
            [Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            sut.Append(GetSafeByteFor(b));
            var position = -1;
            //Act
            TestDelegate callingWithBadParamater = () => sut.Get(position);
            //Assert
            Assert.That(callingWithBadParamater, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void GetByte_ForMultipleAppendedBytes_returnsTheBytes([Random(0, 256, 1)] byte b1,
            [Random(0, 256, 1)] byte b2, [Random(0, 256, 1)] byte b3)
        {
            //Arrange
            var sut = GetSut();
            sut.Append(GetSafeByteFor(b1));
            sut.Append(GetSafeByteFor(b2));
            sut.Append(GetSafeByteFor(b3));
            //Act
            var b1Back = sut.Get(0);
            var b2Back = sut.Get(1);
            var b3Back = sut.Get(2);
            //Assert
            Assert.That(b1Back, Is.EqualTo(b1));
            Assert.That(b2Back, Is.EqualTo(b2));
            Assert.That(b3Back, Is.EqualTo(b3));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public void GetByte_ForSingleAppendedByte_ReturnsByte(byte b)
        {
            //Arrange
            var sut = GetSut();
            var safeByte = GetSafeByteFor(b);
            //Act
            sut.Append(safeByte);
            var safebyteBack = sut.Get(0);
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
            var position = 1;
            //Act
            TestDelegate callingWithBadParamater = () => sut.Get(position);
            //Assert
            Assert.That(callingWithBadParamater, Throws.TypeOf<ArgumentOutOfRangeException>());
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
            var position = 0;
            //Act
            sut.Dispose();
            TestDelegate callingOnDisposedObject = () => sut.Get(position);
            //Assert
            Assert.That(callingOnDisposedObject, Throws.TypeOf<ObjectDisposedException>());
        }

        //** ToDecryptedBytes **//
        [Test]
        public void ToDecryptedBytes_OnEmptyInstance_throwsInvalidOperationException()
        {
            //Arrange
            var sut = GetSut();
            //Act
            TestDelegate callingOnEmptyInstance = () => sut.ToDecryptedBytes();
            //Assert
            Assert.That(callingOnEmptyInstance, Throws.TypeOf<InvalidOperationException>());
        }
    }
}