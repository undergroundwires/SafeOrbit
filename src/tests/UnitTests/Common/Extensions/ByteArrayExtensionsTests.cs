using System;
using System.Linq;
using NUnit.Framework;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Extensions
{
    /// <see cref="ByteArrayExtensions" />
    [TestFixture]
    public class ByteArrayExtensionsTests
    {
        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.DifferentByteArrayPairs32Length))]
        public void Combine_AppendsTheMultipleParametersInRightOrder_ReturnsTrue(byte[] array1, byte[] array2)
        {
            // Arrange
            const int firstIndex = 32;
            var emptyBytes = new byte[firstIndex];

            // Act
            var appendedBytes = emptyBytes.Combine(array1, array2);

            // Assert
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
        public void Combine_AppendsTheParameterAtTheAnd_ReturnsTrue(byte[] array1, byte[] array2)
        {
            // Act
            var newBytes = array1.Combine(array2);

            // Assert
            var sameAsArray2 = new byte[array2.Length];
            Buffer.BlockCopy(newBytes, array1.Length, sameAsArray2, 0, array2.Length);
            var areEqual = sameAsArray2.SequenceEqual(array2);
            Assert.That(areEqual, Is.True);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Combine_ForAnyZeroSizedParameter_CanHandleAppend(byte[] bytes)
        {
            // Arrange
            var zeroSizedParameter = new byte[0];

            // Act
            var newBytes = bytes.Combine(zeroSizedParameter);
            var areEqual = newBytes.SequenceEqual(bytes);

            // Assert
            Assert.That(areEqual, Is.True);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Combine_ForMultipleZeroSizedParameters_CanHandleAppend(byte[] bytes)
        {
            // Arrange
            var zeroSizedParameter = new byte[0];
            var zeroSizedParameter2 = new byte[0];
            var zeroSizedParameter3 = new byte[0];

            // Act
            var newBytes = bytes.Combine(zeroSizedParameter, zeroSizedParameter2, zeroSizedParameter3);
            var areEqual = newBytes.SequenceEqual(bytes);

            // Assert
            Assert.That(areEqual, Is.True);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Combine_ForSingleByteArrayParameter_AppendsToTheObject(byte[] bytes)
        {
            // Arrange
            var expected = bytes.Concat(bytes);

            // Act
            var actual = bytes.Combine(bytes);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Combine_ForZeroSizedObject_CanHandleAppend(byte[] bytes)
        {
            // Arrange
            var zeroSizedObject = new byte[0];

            // Act
            var newBytes = zeroSizedObject.Combine(bytes);
            var areEqual = newBytes.SequenceEqual(bytes);

            // Assert
            Assert.That(areEqual, Is.True);
        }

        [Test]
        public void Combine_ForZeroSizedObjectAndParameters_CanHandleAppend()
        {
            // Arrange
            var zeroSizedObject = new byte[0];
            var zeroSizedParameter = new byte[0];

            // Act
            var newBytes = zeroSizedObject.Combine(zeroSizedParameter);
            var areEqual = newBytes.SequenceEqual(zeroSizedObject);

            // Assert
            Assert.That(areEqual, Is.True);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Combine_IfAnyOfParametersIsNull_ThrowsArgumentNullException(byte[] array)
        {
            // Arrange
            var nullParameter = (byte[]) null;

            // Act
            void UsingNullObject() => array.Combine(array, nullParameter);

            // Assert
            Assert.That(UsingNullObject, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Combine_WhenGivenObjectIsNull_ThrowsArgumentNullException(byte[] bytes)
        {
            // Arrange
            var nullObject = (byte[]) null;

            // Act
            void UsingNullObject() => nullObject.Combine();

            // Assert
            Assert.That(UsingNullObject, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void Combine_WhenParametersAreNull_ThrowsArgumentNullException(byte[] bytes)
        {
            // Arrange
            var nullParameter = (byte[]) null;

            // Act
            void UsingNullObject() => bytes.Combine(nullParameter);

            // Assert
            Assert.That(UsingNullObject, Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void CopyToNewArray_GivenBytes_ReturnsANewByteArray(byte[] oldBytes)
        {
            var newBytes = oldBytes.CopyToNewArray();
            Assert.False(ReferenceEquals(newBytes, oldBytes));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.ByteArray32Length))]
        public void CopyToNewArray_CopiedBytes_AreEquals(byte[] expected)
        {
            var actual = expected.CopyToNewArray();
            Assert.That(actual, Is.EqualTo(expected));
        }


        [Test]
        public void CopyToNewArray_NullBytes_Throws()
        {
            // Arrange
            var nullBytes = (byte[]) null;

            // Act
            void UsingNullObject() => nullBytes.CopyToNewArray();

            // Assert
            Assert.That(UsingNullObject, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void CopyToNewArray_EmptyBytes_ReturnsEmptyByteArray()
        {
            // Arrange
            var emptyBytes = new byte[0];

            // Act
            var actual = emptyBytes.CopyToNewArray();

            // Assert
            Assert.False(ReferenceEquals(emptyBytes, actual));
            Assert.True(actual.Length == 0);
        }
    }
}