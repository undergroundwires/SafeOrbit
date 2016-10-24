
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

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
using SafeOrbit.Memory.SafeBytesServices.Collection;
using SafeOrbit.Memory.SafeBytesServices.Factory;
using SafeOrbit.Random;
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