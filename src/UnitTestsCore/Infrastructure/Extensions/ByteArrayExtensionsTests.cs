
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
using System.Linq;
using NUnit.Framework;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Extensions
{
    /// <see cref="ByteArrayExtensions"/>
    [TestFixture]
    public class ByteArrayExtensionsTests
    {
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentByteArrayPairs32Length))]
        public void Combine_AppendsTheMultipleParametersInRightOrder_returnsTrue(byte[] array1, byte[] array2)
        {
            //Arrange
            const int firstIndex = 32;
            var emptyBytes = new byte[firstIndex];
            //Act
            var appendedBytes = emptyBytes.Combine(array1, array2);
            //Assert
            var sameAsArray1 = new byte[array2.Length];
            Buffer.BlockCopy(appendedBytes, firstIndex, sameAsArray1, 0, array1.Length);
            var sameAsArray2 = new byte[array2.Length];
            Buffer.BlockCopy(appendedBytes, firstIndex + array1.Length, sameAsArray2, 0, array2.Length);
            var firstArrayEqual = sameAsArray1.SequenceEqual(array1);
            var secondArrayEqual = sameAsArray2.SequenceEqual(array2);
            Assert.That(firstArrayEqual, Is.True);
            Assert.That(secondArrayEqual, Is.True);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentByteArrayPairs32Length))]
        public void Combine_AppendsTheParameterAtTheAnd_returnsTrue(byte[] array1, byte[] array2)
        {
            //Act
            var newBytes = array1.Combine(array2);
            //Assert
            var sameAsArray2 = new byte[array2.Length];
            Buffer.BlockCopy(newBytes, array1.Length, sameAsArray2, 0, array2.Length);
            var areEqual = sameAsArray2.SequenceEqual(array2);
            Assert.That(areEqual, Is.True);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Combine_ForAnyZeroSizedParameter_canHandleAppend(byte[] bytes)
        {
            //Arrange
            var zeroSizedParameter = new byte[0];
            //Act
            var newBytes = bytes.Combine(zeroSizedParameter);
            var areEqual = newBytes.SequenceEqual(newBytes);
            //Assert
            Assert.That(areEqual, Is.True);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Combine_ForMultipleZeroSizedParameters_canHandleAppend(byte[] bytes)
        {
            //Arrange
            var zeroSizedParameter = new byte[0];
            var zeroSizedParameter2 = new byte[0];
            var zeroSizedParameter3 = new byte[0];
            //Act
            var newBytes = bytes.Combine(zeroSizedParameter, zeroSizedParameter2, zeroSizedParameter3);
            var areEqual = newBytes.SequenceEqual(newBytes);
            //Assert
            Assert.That(areEqual, Is.True);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Combine_ForSingleByteArrayParameter_appendsToTheObject(byte[] bytes)
        {
            //Arrange
            var expected = bytes.Concat(bytes);
            //Act
            var actual = bytes.Combine(bytes);
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Combine_ForZeroSizedObject_canHandleAppend(byte[] bytes)
        {
            //Arrange
            var zeroSizedObject = new byte[0];
            //Act
            var newBytes = zeroSizedObject.Combine(bytes);
            var areEqual = newBytes.SequenceEqual(newBytes);
            //Assert
            Assert.That(areEqual, Is.True);
        }

        [Test]
        public void Combine_ForZeroSizedObjectAndParameters_canHandleAppend()
        {
            //Arrange
            var zeroSizedObject = new byte[0];
            var zeroSizedParameter = new byte[0];
            //Act
            var newBytes = zeroSizedObject.Combine(zeroSizedParameter);
            var areEqual = newBytes.SequenceEqual(newBytes);
            //Assert
            Assert.That(areEqual, Is.True);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Combine_IfAnyOfParametersIsNull_throwsArgumentNullException(byte[] array)
        {
            //Arrange
            var nullParameter = (byte[]) null;
            //Act
            TestDelegate usingNullObject = () => array.Combine(array, nullParameter);
            //Assert
            Assert.That(usingNullObject, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Combine_WhenGivenObjectIsNull_throwsArgumentNullException(byte[] bytes)
        {
            //Arrange
            var nullObject = (byte[]) null;
            //Act
            TestDelegate usingNullObject = () => nullObject.Combine();
            //Assert
            Assert.That(usingNullObject, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Combine_WhenParametersAreNull_throwsArgumentNullException(byte[] bytes)
        {
            //Arrange
            var nullParameter = (byte[]) null;
            //Act
            TestDelegate usingNullObject = () => bytes.Combine(nullParameter);
            //Assert
            Assert.That(usingNullObject, Throws.TypeOf<ArgumentNullException>());
        }
    }
}