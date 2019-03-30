﻿using System;
using NUnit.Framework;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.SafeBytesServices.Collection;
using SafeOrbit.Memory.SafeBytesServices.Factory;
using SafeOrbit.Tests;
using SafeOrbit.UnitTests;

namespace SafeOrbit.Memory
{
    public class SafeBytesTests : TestsFor<ISafeBytes>
    {
        //** IsNullOrEmpty **//
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

        //** IsDisposed **//
        [Test]
        public void IsDisposed_ForNewSafeBytesInstance_returnsFalse()
        {
            //Arrange
            var sut = GetSut();
            //Act
            var isDisposed = sut.IsDisposed;
            //Assert
            Assert.That(isDisposed, Is.False);
        }

        //** Append(byte) **//
        [Test]
        public void AppendByte_ForDisposedObject_throwsObjectDisposedException([Random(0, 256, 1)] byte b)
        {
            //Arrange
            var sut = GetSut();
            sut.Dispose();
            //Act
            TestDelegate appendingByte = () => sut.Append(b);
            //Act
            Assert.That(appendingByte, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void GetByte_OnEmptyInstance_throwsInvalidOperationException()
        {
            //Arrange
            var sut = GetSut();
            var position = 0;
            //Act
            TestDelegate callingOnEmptyInstance = () => sut.GetByte(position);
            //Assert
            Assert.That(callingOnEmptyInstance, Throws.TypeOf<InvalidOperationException>());
        }

        protected override ISafeBytes GetSut()
        {
            return new SafeBytes(
                Stubs.Get<IFastRandom>(),
                Stubs.Get<ISafeByteFactory>(),
                Stubs.GetFactory<ISafeBytes>(),
                Stubs.GetFactory<ISafeByteCollection>());
        }
    }
}