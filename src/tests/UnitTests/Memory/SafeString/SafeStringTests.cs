using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SafeOrbit.Infrastructure;
using SafeOrbit.Extensions;
using SafeOrbit.Fakes;
using SafeOrbit.Tests;
using SafeOrbit.Text;
using SafeOrbit.UnitTests;

namespace SafeOrbit.Memory
{
    [TestFixture]
    public partial class SafeStringTests
    {
        private static ISafeString GetSut(ITextService textService = null)
        {
            return new SafeString(
                textService ?? Stubs.Get<ITextService>(),
                Stubs.GetFactory<ISafeString>(),
                Stubs.GetFactory<ISafeBytes>());
        }

        [Test]
        public void Clear_CallingTwice_doesNotThrow()
        {
            //Arrange
            var sut = GetSut();
            //Act
            sut.Clear();
            void CallingTwice() => sut.Clear();
            //Assert
            Assert.DoesNotThrow(CallingTwice);
        }

        [Test]
        public void Clear_ForLengthProperty_setsZero()
        {
            //Arrange
            var sut = GetSut();
            sut.Append('A');
            sut.Append('b');
            sut.Append('c');
            var expected = 0;
            //Act
            sut.Clear();
            var actual = sut.Length;
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        //** Clear() **//
        [Test]
        public void Clear_OnDisposedObject_throwsObjectDisposedException()
        {
            //Arrange
            var sut = GetSut();
            sut.Dispose();
            //Act
            void CallOnDisposedObject() => sut.Clear();
            //Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void DeepClone_AppendingToObject_doesNotChangeCloned([Random(0, 256, 1)] int i1,
            [Random(0, 256, 1)] int i2)
        {
            //Arrange
            var sut = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2;
            sut.Append(ch1);
            var expected = sut.Length;
            //Act
            var clone = sut.DeepClone();
            sut.Append(ch2);
            var actual = clone.Length;
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void DeepClone_ClonedInstanceWithSameValue_doesNotReferToSameObject([Random(0, 256, 1)] int i)
        {
            //Arrange
            var sut = GetSut();
            var ch = (char) i;
            sut.Append(ch);
            //Act
            var clone = sut.DeepClone();
            //Act & Assert
            Assert.That(clone, Is.Not.SameAs(sut));
        }

        [Test]
        public void DeepClone_ClonedObjectsToSafeBytes_returnsEqualSafeBytes(
            [Random(0, 256, 1)] int i1,
            [Random(0, 256, 1)] int i2,
            [Random(0, 256, 1)] int i3)
        {
            //Arrange
            var sut = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2, ch3 = (char) i3;
            sut.Append(ch1);
            sut.Append(ch2);
            sut.Append(ch3);
            var expected = sut.ToSafeBytes().ToByteArray();
            //Act
            var clone = sut.DeepClone();
            var actual = clone.ToSafeBytes().ToByteArray();
            var areEqual = expected.SequenceEqual(actual);
            //Assert
            Assert.That(areEqual, Is.True);
        }

        [Test]
        public void DeepClone_ClonedObjectsValue_isEqual([Random(0, 256, 1)] int i1)
        {
            //Arrange
            var sut = GetSut();
            var expected = (char) i1;
            sut.Append(expected);
            var clone = sut.DeepClone();
            //Act
            var actual = clone.GetAsChar(0);
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        //** DeepClone() **//
        [Test]
        public void DeepClone_OnDisposedObject_returnsObjectDisposedException([Random(0, 256, 1)] int i1)
        {
            //Arrange
            var sut = GetSut();
            sut.Append((char) i1);
            //Act
            sut.Dispose();
            void CallOnDisposedObject() => sut.DeepClone();
            //Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void Dispose_AfterInvoked_setsIsDisposedToTrue()
        {
            //Arrange
            var sut = GetSut();
            //Act
            var isNotDisposed = sut.IsDisposed;
            sut.Dispose();
            var isDisposed = sut.IsDisposed;
            //Assert
            Assert.That(isNotDisposed, Is.False);
            Assert.That(isDisposed, Is.True);
        }

        //** Dispose() **//
        [Test]
        public void Dispose_DisposingTwice_throwsObjectDisposedException()
        {
            //Arrange
            var sut = GetSut();
            sut.Dispose();
            //Act
            void DisposeAgain() => sut.Dispose();
            //Assert
            Assert.That(DisposeAgain, Throws.TypeOf<ObjectDisposedException>());
        }

        //** Equals() **//
        [Test]
        public void Equals_ParameterIsNullObject_returnsFalse()
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
        public void EqualsISafeString_ForDifferentInstancesHoldingSameChars_returnsTrue([Random(0, 256, 1)] byte i1,
            [Random(0, 256, 1)] byte i2)
        {
            //Arrange
            var sut = GetSut();
            var holdingSame = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2;
            sut.Append(ch1);
            sut.Append(ch2);
            holdingSame.Append(ch1);
            holdingSame.Append(ch2);
            //Act
            var equals = sut.Equals(holdingSame);
            var equalsOpposite = holdingSame.Equals(sut);
            //Assert
            Assert.That(equals, Is.True);
            Assert.That(equalsOpposite, Is.True);
        }

        [Test]
        public void EqualsISafeString_ForEmptyInstances_returnsTrue()
        {
            //Arrange
            var sut = GetSut();
            var empty = GetSut();
            //Act
            var equals = sut.Equals(empty);
            var equalsOpposite = empty.Equals(sut);
            //Assert
            Assert.That(equals, Is.True);
            Assert.That(equalsOpposite, Is.True);
        }

        [Test]
        public void EqualsISafeString_ForInstancesHoldingDifferentChars_returnsFalse([Random(0, 125, 1)] byte i1,
            [Random(125, 256, 1)] byte i2)
        {
            //Arrange
            var sut = GetSut();
            var holdingDifferent = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2;
            sut.Append(ch1);
            holdingDifferent.Append(ch2);
            //Act
            var equals = sut.Equals(holdingDifferent);
            var equalsOpposite = holdingDifferent.Equals(sut);
            //Assert
            Assert.That(equals, Is.False);
            Assert.That(equalsOpposite, Is.False);
        }

        [Test]
        public void EqualsISafeString_ForInstancesHoldingSameCharsInDifferentOrder_returnsFalse(
            [Random(0, 125, 1)] byte i1, [Random(125, 256, 1)] byte i2)
        {
            //Arrange
            var sut = GetSut();
            var holdingDifferent = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2;
            sut.Append(ch1);
            sut.Append(ch2);
            holdingDifferent.Append(ch2);
            holdingDifferent.Append(ch1);
            //Act
            var equals = sut.Equals(holdingDifferent);
            var equalsOpposite = holdingDifferent.Equals(sut);
            //Assert
            Assert.That(equals, Is.False);
            Assert.That(equalsOpposite, Is.False);
        }

        [Test]
        public void EqualsISafeString_ForSameObjects_returnsTrue([Random(0, 256, 1)] byte i)
        {
            //Arrange
            var sut = GetSut();
            var ch = (char) i;
            sut.Append(ch);
            var sameObject = sut;
            //Act
            var equals = sut.Equals(sameObject);
            var equalsOpposite = sameObject.Equals(sut);
            //Assert
            Assert.That(equals, Is.True);
            Assert.That(equalsOpposite, Is.True);
        }

        [Test]
        public void EqualsISafeString_ParameterIsNullSafeBytesAndSelfIsEmpty_returnsTrue()
        {
            //Arrange
            var sut = GetSut();
            ISafeString nullInstance = null;
            //Act
            var equals = sut.Equals(nullInstance);
            //Assert
            Assert.That(equals, Is.True);
        }

        [Test]
        public void EqualsISafeString_ParameterIsNullSafeBytesAndSelfIsNotEmpty_returnsFalse()
        {
            //Arrange
            var sut = GetSut();
            sut.Append("I'm not empty!");
            ISafeString nullInstance = null;
            //Act
            var equals = sut.Equals(nullInstance);
            //Assert
            Assert.That(equals, Is.False);
        }

        [Test]
        public void EqualsString_WhenHoldingSameChars_returnsTrue()
        {
            //Arrange
            const char ch1 = 'a', ch2 = 'b';
            const string expected = "ab";
            var sut = GetSut();
            sut.Append(ch1);
            sut.Append(ch2);
            //Act
            var equals = sut.Equals(expected);
            //Assert
            Assert.True(equals);
        }

        [Test]
        public void EqualsString_ParameterIsNullStringAndSelfIsEmpty_returnsTrue()
        {
            //Arrange
            var sut = GetSut();
            string nullString = null;
            //Act
            var equals = sut.Equals(nullString);
            //Assert
            Assert.That(equals, Is.True);
        }

        [Test]
        public void EqualsString_ParameterIsNullStringAndSelfIsNotEmpty_returnsFalse()
        {
            //Arrange
            var sut = GetSut();
            sut.Append("I'm not empty!");
            string nullString = null;
            //Act
            var equals = sut.Equals(nullString);
            //Assert
            Assert.That(equals, Is.False);
        }

        [Test]
        public void EqualsStringy_WhenHoldingDifferentChars_returnsFalse([Random(0, 125, 1)] byte i1,
            [Random(125, 256, 1)] byte i2)
        {
            //Arrange
            var sut = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2;
            sut.Append(ch1);
            sut.Append(ch2);
            var text = $"{ch2}{ch1}";
            //Act
            var equals = sut.Equals(text);
            //Assert
            Assert.That(equals, Is.False);
        }

        [Test]
        public void GetHashCode_ForDistinctEmptyInstances_returnsTrue()
        {
            //Arrange
            var sut = GetSut();
            var holdingSame = GetSut();
            var hashCode = sut.GetHashCode();
            var sameHashCode = holdingSame.GetHashCode();
            //Assert
            Assert.That(hashCode, Is.EqualTo(sameHashCode));
        }

        [Test]
        public void GetHashCode_ForDistinctObjectsHavingSameMultipleChars_returnsTrue(
            [Random(0, 256, 1)] int i1, [Random(0, 256, 1)] int i2, [Random(0, 256, 1)] int i3)
        {
            //Arrange
            var sut = GetSut();
            var holdingSame = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2, ch3 = (char) i3;
            sut.Append(ch1);
            sut.Append(ch2);
            sut.Append(ch3);
            holdingSame.Append(ch1);
            holdingSame.Append(ch2);
            holdingSame.Append(ch3);
            //Act
            var hashCode = sut.GetHashCode();
            var sameHashCode = holdingSame.GetHashCode();
            //Assert
            Assert.That(hashCode, Is.EqualTo(sameHashCode));
        }

        //** GetHashCode() **//
        [Test]
        public void GetHashCode_ForDistinctObjectsHavingSameSingleChar_returnsTrue([Random(0, 256, 1)] int i)
        {
            //Arrange
            var sut = GetSut();
            var holdingSame = GetSut();
            var ch = (char) i;
            sut.Append(ch);
            holdingSame.Append(ch);
            //Act
            var hashCode = sut.GetHashCode();
            var sameHashCode = holdingSame.GetHashCode();
            //Assert
            Assert.That(hashCode, Is.EqualTo(sameHashCode));
        }

        [Test]
        public void GetHashCode_ForTwoNonEqualObjects_returnsFalse(
            [Random(0, 256, 1)] int i1, [Random(0, 256, 1)] int i2, [Random(0, 256, 1)] int i3)
        {
            //Arrange
            var sut = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2, ch3 = (char) i3;
            sut.Append(ch1);
            sut.Append(ch2);
            var holdingDifferent = GetSut();
            holdingDifferent.Append(ch3);
            //Act
            var hashCode = sut.GetHashCode();
            var hashCodeForDifferentObject = holdingDifferent.GetHashCode();
            //Assert
            Assert.That(hashCode, Is.Not.EqualTo(hashCodeForDifferentObject));
        }

        [Test]
        public void InsertChar_IndexHigherThanLength_throwsArgumentOutOfRangeException([Random(0, 256, 1)] int i)
        {
            //Arrange
            var sut = GetSut();
            var ch = (char) i;
            sut.Append(ch);
            var index = sut.Length + 1;
            //Act
            void CallOnDisposedObject() => sut.Insert(index, ch);
            //Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void InsertChar_IndexLowerThanZero_throwsArgumentOutOfRangeException([Random(0, 256, 1)] int i)
        {
            //Arrange
            var sut = GetSut();
            var index = -1;
            var c = (char) i;
            sut.Append(c);
            //Act
            void CallOnDisposedObject() => sut.Insert(index, c);
            //Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void InsertChar_InsertingCharsTwice_increasesLength([Random(0, 256, 1)] int i1,
            [Random(0, 256, 1)] int i2)
        {
            //Arrange
            var sut = GetSut();
            var insertPos = 0;
            char c1 = (char) i1, c2 = (char) i2;
            int expected1 = 1, expected2 = 2;
            //Act
            sut.Insert(insertPos, c1);
            var actual1 = sut.Length;
            sut.Insert(insertPos, c2);
            var actual2 = sut.Length;
            //Assert
            Assert.That(actual1, Is.EqualTo(expected1));
            Assert.That(actual2, Is.EqualTo(expected2));
        }

        [Test]
        public void InsertChar_OnDisposedObject_throwsObjectDisposedException([Random(0, 256, 1)] int i)
        {
            //Arrange
            var sut = GetSut();
            var c = (char) i;
            sut.Dispose();
            //Act
            void CallOnDisposedObject() => sut.Insert(0, c);
            //Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void InsertISafeBytes_IndexHigherThanLength_throwsArgumentOutOfRangeException([Random(0, 256, 1)] int i)
        {
            //Arrange
            var sut = GetSut();
            var safeBytes = Stubs.Get<ISafeBytes>().AppendAndReturnDeepClone((byte) i);
            sut.Append(safeBytes);
            var index = sut.Length + 1;
            //Act
            void CallOnDisposedObject() => sut.Insert(index, safeBytes);
            //Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void InsertISafeBytes_IndexLowerThanZero_throwsArgumentOutOfRangeException([Random(0, 256, 1)] int i)
        {
            //Arrange
            var sut = GetSut();
            var index = -1;
            var safeBytes = Stubs.Get<ISafeBytes>().AppendAndReturnDeepClone((byte) i);
            sut.Append(safeBytes);
            //Act
            void CallOnDisposedObject() => sut.Insert(index, safeBytes);
            //Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void InsertISafeBytes_InsertingCharsTwice_increasesLength([Random(0, 256, 1)] int i1,
            [Random(0, 256, 1)] int i2)
        {
            //Arrange
            var sut = GetSut();
            const int insertPos = 0;
            var safeBytes1 = Stubs.Get<ISafeBytes>().AppendAndReturnDeepClone((byte) i1);
            var safeBytes2 = Stubs.Get<ISafeBytes>().AppendAndReturnDeepClone((byte) i2);
            const int expected1 = 1, expected2 = 2;
            //Act
            sut.Insert(insertPos, safeBytes1);
            var actual1 = sut.Length;
            sut.Insert(insertPos, safeBytes2);
            var actual2 = sut.Length;
            //Assert
            Assert.That(actual1, Is.EqualTo(expected1));
            Assert.That(actual2, Is.EqualTo(expected2));
        }

        [Test]
        public void InsertISafeBytes_OnDisposedObject_throwsObjectDisposedException([Random(0, 256, 1)] int i)
        {
            //Arrange
            var sut = GetSut();
            var safeBytes = Stubs.Get<ISafeBytes>().AppendAndReturnDeepClone((byte) i);
            sut.Dispose();
            //Act
            void CallOnDisposedObject() => sut.Insert(0, safeBytes);
            //Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void Append_MultipleCharsAppended_CanGetCharsInOrder()
        {
            //Arrange
            var sut = GetSut();
            const char one = 'h', two = 'e', three = 'l', four = 'l', five = 'o';
            sut.Append(one);
            sut.Append(two);
            sut.Append(three);
            sut.Append(four);
            sut.Append(five);
            //Act
            var actualOne = sut.GetAsChar(0);
            var actualTwo = sut.GetAsChar(1);
            var actualThree = sut.GetAsChar(2);
            var actualFour = sut.GetAsChar(3);
            var actualFive = sut.GetAsChar(4);
            //Assert
            Assert.AreEqual(one, actualOne);
            Assert.AreEqual(two, actualTwo);
            Assert.AreEqual(three, actualThree);
            Assert.AreEqual(four, actualFour);
            Assert.AreEqual(five, actualFive);
        }

        [Test]
        public void Append_StringIsAppended_CanGetCharsInOrder()
        {
            //Arrange
            var sut = GetSut();
            const char one = 'h', two = 'e', three = 'l', four = 'l', five = 'o';
            sut.Append("hello");
            //Act
            var actualOne = sut.GetAsChar(0);
            var actualTwo = sut.GetAsChar(1);
            var actualThree = sut.GetAsChar(2);
            var actualFour = sut.GetAsChar(3);
            var actualFive = sut.GetAsChar(4);
            //Assert
            Assert.AreEqual(one, actualOne);
            Assert.AreEqual(two, actualTwo);
            Assert.AreEqual(three, actualThree);
            Assert.AreEqual(four, actualFour);
            Assert.AreEqual(five, actualFive);
        }

        [Test]
        public void Append_AsciiText_BytesAreConverted()
        {
            // arrange
            var textServiceMock = new Mock<ITextService>();
            var asciiBytes = new byte[] { 104, 101, 108, 108, 111 }; /* hello */
            var expected = new byte[] { 104, 0, 101, 0, 108, 0, 108, 0, 111, 0 }; /*hello*/
            textServiceMock
                .Setup(s => s.Convert(Encoding.Ascii, Encoding.Utf16LittleEndian, asciiBytes))
                .Returns(expected.ToArray);
            var sut = GetSut(textService: textServiceMock.Object);
            var safeBytes = Stubs.Get<ISafeBytes>();
            foreach(var @byte in asciiBytes)
                safeBytes.Append(@byte);
            // act
            sut.Append(safeBytes, Encoding.Ascii);
            var actual = sut.ToSafeBytes().ToByteArray();
            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Append_Utf16BigEndian_BytesAreConverted()
        {
            // arrange
            var textServiceMock = new Mock<ITextService>();
            var utf16BigEndianBytes = new byte[] { 0, 104, 0, 101, 0, 108, 0, 108, 0, 111 };
            var expected = new byte[] { 104, 0, 101, 0, 108, 0, 108, 0, 111, 0 }; /*hello*/
            textServiceMock
                .Setup(s => s.Convert(Encoding.Utf16BigEndian, Encoding.Utf16LittleEndian, utf16BigEndianBytes))
                .Returns(expected.ToArray);
            var sut = GetSut(textService: textServiceMock.Object);
            var safeBytes = Stubs.Get<ISafeBytes>();
            foreach (var @byte in utf16BigEndianBytes)
                safeBytes.Append(@byte);
            // act
            sut.Append(safeBytes, Encoding.Utf16BigEndian);
            var actual = sut.ToSafeBytes().ToByteArray();
            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Append_Utf16LittleEndian_BytesAreSame()
        {
            // arrange
            var textServiceMock = new Mock<ITextService>();
            var expected = new byte[] { 104, 0, 101, 0, 108, 0, 108, 0, 111, 0 }; /*hello*/
            var sut = GetSut(textService: textServiceMock.Object);
            var safeBytes = Stubs.Get<ISafeBytes>();
            foreach (var @byte in expected)
                safeBytes.Append(@byte);
            // act
            sut.Append(safeBytes, Encoding.Utf16LittleEndian);
            var actual = sut.ToSafeBytes().ToByteArray();
            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsNullOrEmpty_ForDisposedSafeBytesObject_returnsTrue(
            [Random(0, 256, 1)] int i)
        {
            //Arrange
            var sut = GetSut();
            var c = (char) i;
            sut.Append(c);
            sut.Dispose();
            //Act
            var isNull = SafeOrbit.Memory.SafeString.IsNullOrEmpty(sut);
            //Assert
            Assert.That(isNull, Is.True);
        }

        [Test]
        public void IsNullOrEmpty_ForClearedInstance_returnsTrue()
        {
            //Arrange
            var sut = GetSut();
            sut.Append("Hello world");
            sut.Clear();
            //Act
            var isNull = SafeString.IsNullOrEmpty(sut);
            //Assert
            Assert.That(isNull, Is.True);
        }

        [Test]
        public void IsNullOrEmpty_ForNewSafeStringInstance_returnsTrue()
        {
            //Arrange
            var sut = GetSut();
            //Act
            var isEmpty = SafeString.IsNullOrEmpty(sut);
            //Assert
            Assert.That(isEmpty, Is.True);
        }

        [Test]
        public void IsNullOrEmpty_ForNullSafeStringObject_returnsTrue()
        {
            //Arrange
            var nullString = (ISafeString) null;
            //Act
            var isNull = SafeString.IsNullOrEmpty(nullString);
            //Assert
            Assert.That(isNull, Is.True);
        }

        [Test]
        public void IsNullOrEmpty_ForObjectHoldingMultipleBytes_returnsFalse(
            [Random(0, 256, 1)] int i1, [Random(0, 256, 1)] int i2, [Random(0, 256, 1)] int i3)
        {
            //Arrange
            var sut = GetSut();
            char c1 = (char) i1, c2 = (char) i2, c3 = (char) i3;
            sut.Append(c1);
            sut.Append(c2);
            sut.Append(c3);
            //Act
            var isNull = SafeString.IsNullOrEmpty(sut);
            //Assert
            Assert.That(isNull, Is.False);
        }

        [Test]
        public void IsNullOrEmpty_ForObjectHoldingSingleByte_returnsFalse(
            [Random(0, 256, 1)] int i)
        {
            //Arrange
            var sut = GetSut();
            var c = (char) i;
            sut.Append(c);
            //Act
            var isEmpty = SafeString.IsNullOrEmpty(sut);
            //Assert
            Assert.That(isEmpty, Is.False);
        }

        //** Length **//
        [Test]
        public void Length_ForAFreshInstance_isZero()
        {
            //Arrange
            var sut = GetSut();
            //Act
            var length = sut.Length;
            //Assert
            Assert.That(length, Is.EqualTo(0));
        }

        [Test]
        public void Remove_CountParameterIsLessThanOne_throwsArgumentOutOfRangeException([Random(0, 256, 1)] int i)
        {
            //Arrange
            var sut = GetSut();
            var ch = (char) i;
            sut.Append(ch);
            var index = sut.Length;
            //Act
            void CallWithZeroParameter() => sut.Remove(index, 0);
            void CallWithNegativeParameter() => sut.Remove(index, -1);
            //Assert
            Assert.That(CallWithZeroParameter, Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(CallWithNegativeParameter, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Remove_IndexParameterHigherThanLength_throwsArgumentOutOfRangeException([Random(0, 256, 1)] int i)
        {
            //Arrange
            var sut = GetSut();
            var ch = (char) i;
            sut.Append(ch);
            var index = sut.Length + 1;
            //Act
            void CallOnDisposedObject() => sut.Remove(index);
            //Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Remove_IndexParameterLowerThanZero_throwsArgumentOutOfRangeException([Random(0, 256, 1)] int i)
        {
            //Arrange
            var sut = GetSut();
            var index = -1;
            var c = (char) i;
            sut.Append(c);
            //Act
            void CallOnDisposedObject() => sut.Remove(index);
            //Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        //** Remove() **//
        [Test]
        public void Remove_OnDisposedObject_throwsObjectDisposedException([Random(0, 256, 1)] int i)
        {
            //Arrange
            var sut = GetSut();
            var c = (char) i;
            sut.Dispose();
            //Act
            void CallOnDisposedObject() => sut.Remove(0, c);
            //Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void Remove_OtherCharacters_DoesNotAffect(int count)
        {
            //Arrange
            var sut = GetSut();
            const int startIndex = 1;
            IList<char> chars = new[] {'t', 'e', 's', 't'}.ToList();
            foreach (var c in chars)
                sut.Append(c);
            for (var i = 0; i < count; i++)
                chars.RemoveAt(startIndex);
            //Act
            sut.Remove(startIndex, count);
            //Assert
            for (var i = 0; i < sut.Length; i++)
            {
                var actual = sut.GetAsChar(i);
                var expected = chars.ElementAt(i);
                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public void Remove_TotalOfIndexAndCountIsHigherThanLength_throwsArgumentOutOfRangeException()
        {
            //Arrange
            var sut = GetSut();
            const char ch1 = 't', ch2 = 'e';
            sut.Append(ch1);
            sut.Append(ch2);
            var index = sut.Length - 1;
            var count = sut.Length;
            //Act
            void SendBadParameters() => sut.Remove(index, count);
            //Assert
            Assert.That(SendBadParameters, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void Remove_WhenRemoved_decreasesLength(int count)
        {
            //Arrange
            var sut = GetSut();
            const char ch1 = 't', ch2 = 'e';
            sut.Append(ch1);
            sut.Append(ch2);
            var expected = sut.Length - count;
            //Act
            sut.Remove(0, count);
            var actual = sut.Length;
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShallowClone_AppendingToObject_changesCloned([Random(0, 256, 1)] int i1, [Random(0, 256, 1)] int i2)
        {
            //Arrange
            var sut = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2;
            sut.Append(ch1);
            //Act
            var clone = sut.ShallowClone();
            sut.Append(ch2);
            var actual = clone.Length;
            var expected = sut.Length;
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShallowClone_ClonedObjectsToSafeBytes_returnsEqualSafeBytes([Random(0, 256, 1)] int i1,
            [Random(0, 256, 1)] int i2, [Random(0, 256, 1)] int i3)
        {
            //Arrange
            var sut = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2, ch3 = (char) i3;
            sut.Append(ch1);
            sut.Append(ch2);
            sut.Append(ch3);
            var expected = sut.ToSafeBytes().ToByteArray();
            //Act
            var clone = sut.ShallowClone();
            var actual = clone.ToSafeBytes().ToByteArray();
            var areEqual = expected.SequenceEqual(actual);
            //Assert
            Assert.That(areEqual, Is.True);
        }

        [Test]
        public void ShallowClone_ClonedObjectsValue_isEqual([Random(0, 256, 1)] int i1)
        {
            //Arrange
            var sut = GetSut();
            var expected = (char) i1;
            sut.Append(expected);
            var clone = sut.ShallowClone();
            //Act
            var actual = clone.GetAsChar(0);
            //Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        //** ShallowClone() **//
        [Test]
        public void ShallowClone_OnDisposedObject_returnsObjectDisposedException([Random(0, 256, 1)] int i1)
        {
            //Arrange
            var sut = GetSut();
            sut.Append((char) i1);
            //Act
            sut.Dispose();
            void CallOnDisposedObject() => sut.ShallowClone();
            //Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test] 
        public void ToSafeBytes_ChangingReturnedResult_OriginalObject()
        {
            //Arrange
            var sut = GetSut();
            var expected = 'a';
            var index = 0;
            sut.Append(expected);
            //Act
            var bytes = sut.ToSafeBytes();
            bytes.Append(5);
            var actual = sut.GetAsChar(index);
            //Assert
            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public void ToSafeBytes_DisposingReturnedResult_DoesNotAffectOriginalObject()
        {
            //Arrange
            var sut = GetSut();
            const char expected = 'a';
            const int index = 0;
            sut.Append(expected);
            //Act
            var bytes = sut.ToSafeBytes();
            bytes.Dispose();
            var actual = sut.GetAsChar(index);
            //Assert
            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public void ToSafeBytes_ForMultipleChars_returnsEqualBytes()
        {
            //Arrange
            var sut = GetSut();
            sut.Append('a');
            sut.Append('b');
            var charBytes1 = sut.GetAsSafeBytes(0).ToByteArray();
            var charBytes2 = sut.GetAsSafeBytes(1).ToByteArray();
            var expected = charBytes1.Combine(charBytes2);
            //Act
            var actual = sut.ToSafeBytes().ToByteArray();
            //Assert
            var areSame = expected.SequenceEqual(actual);
            Assert.That(areSame, Is.True);
        }

        [Test]
        public void ToSafeBytes_ForSingleChar_returnsEqualBytes()
        {
            //Arrange
            var sut = GetSut();
            sut.Append('a');
            var expected = sut.GetAsSafeBytes(0).ToByteArray();
            //Act
            var actual = sut.ToSafeBytes().ToByteArray();
            //Assert
            var areSame = expected.SequenceEqual(actual);
            Assert.That(areSame, Is.True);
        }

        [Test]
        public void ToSafeBytes_OnDisposedObject_throwsObjectDisposedException()
        {
            //Arrange
            var sut = GetSut();
            sut.Append('a');
            //Act
            sut.Dispose();
            void CallOnDisposedObject() => sut.ToSafeBytes();
            //Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void ToSafeBytes_OnEmptyInstance_throwsInvalidOperationException()
        {
            //Arrange
            var sut = GetSut();
            //Act
            void CallOnDisposedObject() => sut.ToSafeBytes();
            //Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<InvalidOperationException>());
        }
    }
}