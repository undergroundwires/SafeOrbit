using System;
using NUnit.Framework;
using SafeOrbit.Encryption;
using SafeOrbit.Memory;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Memory.SafeBytesServices.Id;
using SafeOrbit.Random;
using SafeOrbit.Tests;
using SafeOrbit.Tests.Cases;
using SafeOrbit.UnitTests;

namespace SafeOrbit.Memory.SafeBytesServices
{
    /// <seealso cref="ISafeByte"/>
    /// <seealso cref="SafeByte"/>
    [TestFixture]
    internal class SafeByteTests : TestsFor<ISafeByte>
    {
        protected override ISafeByte GetSut()
        {
            return new SafeByte
            (
                Stubs.Get<IFastEncryptor>(),
                Stubs.Get<IFastRandom>(),
                Stubs.Get<IByteIdGenerator>(),
                Stubs.Get<IByteArrayProtector>()
            );
        }

        [Test]
        public void DeepClone_ClonedInstanceWithSameValue_doesNotReferToSameObject([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            sut.Set(b);
            //Act & Assert
            Assert.That(sut.DeepClone(), Is.Not.SameAs(sut));
        }

        [Test]
        public void DeepClone_ClonedObjectsGet_returnsEqualByte([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
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
            var sut = GetSut();
            //Act
            TestDelegate callingDeepClone = () => sut.DeepClone();
            //Assert
            Assert.That(callingDeepClone, Throws.TypeOf<InvalidOperationException>());
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
        public void Equals_ForTwoEmptyInstances_returnsTrue()
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
        public void Equals_ObjectWithoutByteWithByte_returnsFalse([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            //Act
            var equals = sut.Equals(b);
            //Assert
            Assert.That(equals, Is.False);
        }

        //** Equals() **//
        [Test]
        public void Equals_WhenParameterIsNullObject_returnsFalse()
        {
            //Arrange
            var sut = GetSut();
            object nullInstance = null;
            //Act
            var equals = sut.Equals(nullInstance);
            //Assert
            Assert.That(equals, Is.False);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentBytePairs))]
        public void EqualsByte_ForDifferentBytes_returnsFalse(byte b1, byte b2)
        {
            //Arrange
            var sut = GetSut();
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
            var sut = GetSut();
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
            var sut = GetSut();
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
        public void EqualsSafeByte_WhenParameterIsNullSafeByte_returnsFalse()
        {
            //Arrange
            var sut = GetSut();
            ISafeByte nullInstance = null;
            //Act
            var equals = sut.Equals(nullInstance);
            //Assert
            Assert.That(equals, Is.False);
        }

        //** Get() **//
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public void Get_ReturnsThePreviouslySetByte_returnsTrue(byte b)
        {
            //Arrange
            var sut = GetSut();
            //Act
            sut.Set(b);
            var byteBack = sut.Get();
            //Assert
            Assert.That(b, Is.EqualTo(byteBack));
        }

        [Test]
        public void Get_WhenCalledForNotSetObject_throwsInvalidOperationException()
        {
            //Arrange
            var sut = GetSut();
            //Act
            TestDelegate callingGet = () => sut.Get();
            //Assert
            Assert.That(callingGet, Throws.TypeOf<InvalidOperationException>());
        }

        //** GetHashCode() **//
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public void GetHashCode_ForDistinctObjectsHavingSameValue_returnsTrue(byte b)
        {
            //Arrange
            var sut = GetSut();
            var holdingSame = GetSut();
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
            var sut = GetSut();
            var holdingDifferent = GetSut();
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
            var sut = GetSut();
            int temp;
            //Act
            TestDelegate gettingIdWithoutSettingByte = () => temp = sut.Id;
            //Act
            Assert.That(gettingIdWithoutSettingByte, Throws.TypeOf<InvalidOperationException>());
        }

        //** IsByteSet **//
        [Test]
        public void IsByteSet_BeforeInvokingSetMethod_returnsFalse()
        {
            //Arrange
            var sut = GetSut();
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
            var sut = GetSut();
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
            var sut = GetSut();
            //Act
            sut.Set(b1);
            TestDelegate callingOneMoreTime = () => sut.Set(b2);
            //Assert
            Assert.That(callingOneMoreTime, Throws.TypeOf<InvalidOperationException>());
        }
    }
}