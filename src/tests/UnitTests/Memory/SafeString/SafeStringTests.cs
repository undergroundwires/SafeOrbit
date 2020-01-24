using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SafeOrbit.Extensions;
using SafeOrbit.Fakes;
using SafeOrbit.Text;

namespace SafeOrbit.Memory
{
    /// <seealso cref="ISafeString" />
    /// <seealso cref="SafeString" />
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
        public void Clear_CallingTwice_DoesNotThrow()
        {
            // Arrange
            using var sut = GetSut();
            
            // Act
            sut.Clear();
            void CallingTwice() => sut.Clear();
            
            // Assert
            Assert.DoesNotThrow(CallingTwice);
        }

        [Test]
        public async Task Clear_ForLengthProperty_SetsZero()
        {
            // Arrange
            using var sut = GetSut();
            await sut.AppendAsync('A');
            await sut.AppendAsync('b');
            await sut.AppendAsync('c');
            const int expected = 0;
            
            // Act
            sut.Clear();
            var actual = sut.Length;
            
            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Clear_OnDisposedObject_ThrowsObjectDisposedException()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();

            // Act
            void CallOnDisposedObject() => sut.Clear();
            
            // Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public async Task DeepCloneAsync_AppendingToObject_DoesNotChangeCloned([Random(0, 256, 1)] int i1,
            [Random(0, 256, 1)] int i2)
        {
            // Arrange
            using var sut = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2;
            await sut.AppendAsync(ch1);
            var expected = sut.Length;
            
            // Act
            var clone = await sut.DeepCloneAsync();
            await sut.AppendAsync(ch2);
            var actual = clone.Length;
            
            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task DeepCloneAsync_ClonedInstanceWithSameValue_DoesNotReferToSameObject([Random(0, 256, 1)] int i)
        {
            // Arrange
            var sut = GetSut();
            var ch = (char) i;
            await sut.AppendAsync(ch);
            
            // Act
            var clone = await sut.DeepCloneAsync();
            
            // Act & Assert
            Assert.That(clone, Is.Not.SameAs(sut));
        }

        [Test]
        public async Task DeepCloneAsync_ClonedObjectsToSafeBytes_ReturnsEqualSafeBytes()
        {
            // Arrange
            using var sut = GetSut();
            char ch1 = 'k', ch2 = 'u', ch3 = 'k';
            await sut.AppendAsync(ch1);
            await sut.AppendAsync(ch2);
            await sut.AppendAsync(ch3);
            var expected = await  (await sut.ToSafeBytesAsync()).ToByteArrayAsync();
            
            // Act
            var clone = await sut.DeepCloneAsync();
            var actual = await (await clone.ToSafeBytesAsync()).ToByteArrayAsync();
            var areEqual = expected.SequenceEqual(actual);
            
            // Assert
            Assert.That(areEqual, Is.True);
        }

        [Test]
        public async Task DeepCloneAsync_ClonedObjectsValue_IsEqual([Random(0, 256, 1)] int i1)
        {
            // Arrange
            using var sut = GetSut();
            var expected = (char) i1;
            await sut.AppendAsync(expected);
            var clone = await sut.DeepCloneAsync();
            
            // Act
            var actual = await clone.GetAsCharAsync(0);
            
            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task DeepCloneAsync_OnDisposedObject_ReturnsObjectDisposedException([Random(0, 256, 1)] int i1)
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync((char) i1);
            
            // Act
            sut.Dispose();
            Task CallOnDisposedObject() => sut.DeepCloneAsync();
            
            // Assert
            Assert.ThrowsAsync<ObjectDisposedException>(CallOnDisposedObject);
        }

        [Test]
        public void DeepCloneAsync_Disposed_Throws()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();

            // Act
            Task<ISafeString> DeepCloneAsync() => sut.DeepCloneAsync();
            
            // Assert
            Assert.ThrowsAsync<ObjectDisposedException>(DeepCloneAsync);
        }

        [Test]
        public void Dispose_AfterInvoked_SetsIsDisposedToTrue()
        {
            // Arrange
            var sut = GetSut();
            
            // Act
            var isNotDisposed = sut.IsDisposed;
            sut.Dispose();
            var isDisposed = sut.IsDisposed;
            
            // Assert
            Assert.That(isNotDisposed, Is.False);
            Assert.That(isDisposed, Is.True);
        }

        //** Dispose() **//
        [Test]
        public void Dispose_DisposingTwice_ThrowsObjectDisposedException()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();

            // Act
            void DisposeAgain() => sut.Dispose();
            
            // Assert
            Assert.That(DisposeAgain, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public async Task InsertAsyncChar_IndexHigherThanLength_ThrowsArgumentOutOfRangeException([Random(0, 256, 1)] int i)
        {
            // Arrange
            using var sut = GetSut();
            var ch = (char) i;
            await sut.AppendAsync(ch);
            var index = sut.Length + 1;

            // Act
            Task CallOnDisposedObject() => sut.InsertAsync(index, ch);
            
            // Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public async Task InsertAsyncChar_IndexLowerThanZero_ThrowsArgumentOutOfRangeException([Random(0, 256, 1)] int i)
        {
            // Arrange
            using var sut = GetSut();
            const int index = -1;
            var c = (char) i;
            await sut.AppendAsync(c);

            // Act
            Task CallOnDisposedObject() => sut.InsertAsync(index, c);
            
            // Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(CallOnDisposedObject);
        }

        [Test]
        public async Task InsertAsyncChar_InsertingCharsTwice_IncreasesLength([Random(0, 256, 1)] int i1,
            [Random(0, 256, 1)] int i2)
        {
            // Arrange
            using var sut = GetSut();
            const int insertPos = 0;
            char c1 = (char) i1, c2 = (char) i2;
            const int expected1 = 1;
            const int expected2 = 2;

            // Act
            await sut.InsertAsync(insertPos, c1);
            var actual1 = sut.Length;
            await sut.InsertAsync(insertPos, c2);
            var actual2 = sut.Length;

            // Assert
            Assert.That(actual1, Is.EqualTo(expected1));
            Assert.That(actual2, Is.EqualTo(expected2));
        }

        [Test]
        public void InsertAsyncChar_OnDisposedObject_ThrowsObjectDisposedException([Random(0, 256, 1)] int i)
        {
            // Arrange
            var sut = GetSut();
            var c = (char) i;
            sut.Dispose();

            // Act
            Task CallOnDisposedObject() => sut.InsertAsync(0, c);
            
            // Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public async Task InsertAsyncISafeBytes_IndexHigherThanLength_ThrowsArgumentOutOfRangeException(
            [Random(0, 256, 1)] int i)
        {
            // Arrange
            using var sut = GetSut();
            var safeBytes = Stubs.Get<ISafeBytes>();
            await safeBytes.AppendAsync((byte) i);
            await sut.AppendAsync(safeBytes);
            var index = sut.Length + 1;

            // Act
            Task CallOnDisposedObject() => sut.InsertAsync(index, safeBytes);

            // Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(CallOnDisposedObject);
        }

        [Test]
        public async Task InsertAsyncISafeBytes_IndexLowerThanZero_ThrowsArgumentOutOfRangeException([Random(0, 256, 1)] int i)
        {
            // Arrange
            using var sut = GetSut();
            const int index = -1;
            var safeBytes = Stubs.Get<ISafeBytes>();
            await safeBytes.AppendAsync((byte) i);
            await sut.AppendAsync(safeBytes);

            // Act
            Task CallOnDisposedObject() => sut.InsertAsync(index, safeBytes);
            
            // Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(CallOnDisposedObject);
        }

        [Test]
        public async Task InsertAsyncISafeBytes_InsertingCharsTwice_IncreasesLength()
        {
            // Arrange
            using var sut = GetSut();
            const int insertPos = 0;
            var safeBytes1 = Stubs.Get<ISafeBytes>();
            await safeBytes1.AppendAsync( 11);
            var safeBytes2 = Stubs.Get<ISafeBytes>();
            await safeBytes2.AppendAsync(12);
            const int expected1 = 1, expected2 = 2;
            
            // Act
            await sut.InsertAsync(insertPos, safeBytes1);
            var actual1 = sut.Length;
            await sut.InsertAsync(insertPos, safeBytes2);
            var actual2 = sut.Length;
            
            // Assert
            Assert.That(actual1, Is.EqualTo(expected1));
            Assert.That(actual2, Is.EqualTo(expected2));
        }

        [Test]
        public async Task InsertAsyncISafeBytes_OnDisposedObject_ThrowsObjectDisposedException([Random(0, 256, 1)] int i)
        {
            // Arrange
            var sut = GetSut();
            var safeBytes = Stubs.Get<ISafeBytes>();
            await safeBytes.AppendAsync((byte)i);
            sut.Dispose();

            // Act
            Task CallOnDisposedObject() => sut.InsertAsync(0, safeBytes);
            
            // Assert
            Assert.ThrowsAsync<ObjectDisposedException>(CallOnDisposedObject);
        }

        [Test]
        public void AppendAsyncChar_DisposedObject_ThrowsObjectDisposedException()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();

            // Act
            Task CallOnDisposedObject() => sut.AppendAsync('e');

            // Assert
            Assert.ThrowsAsync<ObjectDisposedException>(CallOnDisposedObject);
        }

        [Test]
        public async Task AppendChar_MultipleCharsAppended_CanGetCharsInOrder()
        {
            // Arrange
            using var sut = GetSut();
            const char one = 'h', two = 'e', three = 'l', four = 'l', five = 'o';
            await sut.AppendAsync(one);
            await sut.AppendAsync(two);
            await sut.AppendAsync(three);
            await sut.AppendAsync(four);
            await sut.AppendAsync(five);
            
            // Act
            var actualOne = await sut.GetAsCharAsync(0);
            var actualTwo = await sut.GetAsCharAsync(1);
            var actualThree = await sut.GetAsCharAsync(2);
            var actualFour = await sut.GetAsCharAsync(3);
            var actualFive = await sut.GetAsCharAsync(4);
            
            // Assert
            Assert.AreEqual(one, actualOne);
            Assert.AreEqual(two, actualTwo);
            Assert.AreEqual(three, actualThree);
            Assert.AreEqual(four, actualFour);
            Assert.AreEqual(five, actualFive);
        }

        [Test]
        public void AppendAsyncString_DisposedObject_ThrowsObjectDisposedException()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();

            // Act
            Task CallOnDisposedObject() => sut.AppendAsync("hello");
            
            // Assert
            Assert.ThrowsAsync<ObjectDisposedException>(CallOnDisposedObject);
        }

        [Test]
        public async Task AppendAsyncString_StringIsAppended_CanGetCharsInOrder()
        {
            // Arrange
            using var sut = GetSut();
            const char one = 'h', two = 'e', three = 'l', four = 'l', five = 'o';
            await sut.AppendAsync("hello");
            
            // Act
            var actualOne = await sut.GetAsCharAsync(0);
            var actualTwo = await sut.GetAsCharAsync(1);
            var actualThree = await sut.GetAsCharAsync(2);
            var actualFour = await sut.GetAsCharAsync(3);
            var actualFive = await sut.GetAsCharAsync(4);
            
            // Assert
            Assert.AreEqual(one, actualOne);
            Assert.AreEqual(two, actualTwo);
            Assert.AreEqual(three, actualThree);
            Assert.AreEqual(four, actualFour);
            Assert.AreEqual(five, actualFive);
        }

        [Test]
        public async Task AppendAsyncBytes_AsciiText_BytesAreConverted()
        {
            // Arrange
            var textServiceMock = new Mock<ITextService>();
            var asciiBytes = new byte[] {104, 101, 108, 108, 111}; /* hello */
            var expected = new byte[] {104, 0, 101, 0, 108, 0, 108, 0, 111, 0}; /*hello*/
            textServiceMock
                .Setup(s => s.Convert(Encoding.Ascii, Encoding.Utf16LittleEndian, asciiBytes))
                .Returns(expected.ToArray);
            using var sut = GetSut(textService: textServiceMock.Object);
            var safeBytes = Stubs.Get<ISafeBytes>();
            foreach (var @byte in asciiBytes)
                await safeBytes.AppendAsync(@byte);

            // Act
            await sut.AppendAsync(safeBytes, Encoding.Ascii);
            var actual = await (await sut.ToSafeBytesAsync()).ToByteArrayAsync();
            
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task AppendAsyncBytes_Utf16BigEndian_BytesAreConverted()
        {
            // Arrange
            var textServiceMock = new Mock<ITextService>();
            var utf16BigEndianBytes = new byte[] {0, 104, 0, 101, 0, 108, 0, 108, 0, 111};
            var expected = new byte[] {104, 0, 101, 0, 108, 0, 108, 0, 111, 0}; /*hello*/
            textServiceMock
                .Setup(s => s.Convert(Encoding.Utf16BigEndian, Encoding.Utf16LittleEndian, utf16BigEndianBytes))
                .Returns(expected.ToArray);
            using var sut = GetSut(textService: textServiceMock.Object);
            var safeBytes = Stubs.Get<ISafeBytes>();
            foreach (var @byte in utf16BigEndianBytes)
                await safeBytes.AppendAsync(@byte);

            // Act
            await sut.AppendAsync(safeBytes, Encoding.Utf16BigEndian);
            var actual = await (await sut.ToSafeBytesAsync()).ToByteArrayAsync();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task AppendAsyncBytes_Utf16LittleEndian_BytesAreSame()
        {
            // Arrange
            var textServiceMock = new Mock<ITextService>();
            var expected = new byte[] {104, 0, 101, 0, 108, 0, 108, 0, 111, 0}; /*hello*/
            using var sut = GetSut(textService: textServiceMock.Object);
            var safeBytes = Stubs.Get<ISafeBytes>();
            foreach (var @byte in expected)
                await safeBytes.AppendAsync(@byte);
           
            // Act
            await sut.AppendAsync(safeBytes);
            var actual = await(await sut.ToSafeBytesAsync()).ToByteArrayAsync();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task AppendAsyncBytes_DisposedObject_ThrowsObjectDisposedException()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();
            var safeBytes = Stubs.Get<ISafeBytes>();
            await safeBytes.AppendAsync(55);

            // Act
            Task CallOnDisposedObject() => sut.AppendAsync(safeBytes);

            // Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void AppendLineAsync_DisposedObject_ThrowsObjectDisposedException()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();

            // Act
            Task CallOnDisposedObject() => sut.AppendLineAsync();

            // Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public async Task AppendLineAsync_AppendsUnicodeLineFeed()
        {
            // Arrange
            const int expected = 0x000A;
            using var sut = GetSut();

            // Act
            await sut.AppendLineAsync();

            // Assert
            var actual = await sut.GetAsSafeBytes(0).GetByteAsync(0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task IsNullOrEmpty_ForDisposedSafeBytesObject_ReturnsTrue(
            [Random(0, 256, 1)] int i)
        {
            // Arrange
            var sut = GetSut();
            var c = (char) i;
            await sut.AppendAsync(c);
            sut.Dispose();
            
            // Act
            var isNull = SafeString.IsNullOrEmpty(sut);
            
            // Assert
            Assert.That(isNull, Is.True);
        }

        [Test]
        public async Task IsNullOrEmpty_ForClearedInstance_ReturnsTrue()
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync("Hello world");
            sut.Clear();

            // Act
            var isNull = SafeString.IsNullOrEmpty(sut);
            
            // Assert
            Assert.That(isNull, Is.True);
        }

        [Test]
        public void IsNullOrEmpty_ForNewSafeStringInstance_ReturnsTrue()
        {
            // Arrange
            var sut = GetSut();
            
            // Act
            var isEmpty = SafeString.IsNullOrEmpty(sut);
            
            // Assert
            Assert.That(isEmpty, Is.True);
        }

        [Test]
        public void IsNullOrEmpty_ForNullSafeStringObject_ReturnsTrue()
        {
            // Arrange
            var nullString = (ISafeString) null;
            
            // Act
            var isNull = SafeString.IsNullOrEmpty(nullString);
            
            // Assert
            Assert.That(isNull, Is.True);
        }

        [Test]
        public async Task IsNullOrEmpty_ForObjectHoldingMultipleBytes_ReturnsFalse(
            [Random(0, 256, 1)] int i1, [Random(0, 256, 1)] int i2, [Random(0, 256, 1)] int i3)
        {
            // Arrange
            var sut = GetSut();
            char c1 = (char) i1, c2 = (char) i2, c3 = (char) i3;
            await sut.AppendAsync(c1);
            await sut.AppendAsync(c2);
            await sut.AppendAsync(c3);
            
            // Act
            var isNull = SafeString.IsNullOrEmpty(sut);
            
            // Assert
            Assert.That(isNull, Is.False);
        }

        [Test]
        public async Task IsNullOrEmpty_ForObjectHoldingSingleByte_ReturnsFalse(
            [Random(0, 256, 1)] int i)
        {
            // Arrange
            using var sut = GetSut();
            var c = (char) i;
            await sut.AppendAsync(c);

            // Act
            var isEmpty = SafeString.IsNullOrEmpty(sut);
            
            // Assert
            Assert.That(isEmpty, Is.False);
        }

        //** Length **//
        [Test]
        public void Length_ForAFreshInstance_isZero()
        {
            // Arrange
            using var sut = GetSut();

            // Act
            var length = sut.Length;

            // Assert
            Assert.That(length, Is.EqualTo(0));
        }

        [Test]
        public async Task Remove_CountParameterIsLessThanOne_ThrowsArgumentOutOfRangeException([Random(0, 256, 1)] int i)
        {
            // Arrange
            using var sut = GetSut();
            var ch = (char) i;
            await sut.AppendAsync(ch);
            var index = sut.Length;

            // Act
            void CallWithZeroParameter() => sut.Remove(index, 0);
            void CallWithNegativeParameter() => sut.Remove(index, -1);
            
            // Assert
            Assert.That(CallWithZeroParameter, Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(CallWithNegativeParameter, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public async Task Remove_IndexParameterHigherThanLength_ThrowsArgumentOutOfRangeException([Random(0, 256, 1)] int i)
        {
            // Arrange
            using var sut = GetSut();
            var ch = (char) i;
            await sut.AppendAsync(ch);
            var index = sut.Length + 1;

            // Act
            void CallOnDisposedObject() => sut.Remove(index);
            
            // Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public async Task Remove_IndexParameterLowerThanZero_ThrowsArgumentOutOfRangeException([Random(0, 256, 1)] int i)
        {
            // Arrange
            using var sut = GetSut();
            const int index = -1;
            var c = (char) i;
            await sut.AppendAsync(c);

            // Act
            void CallOnDisposedObject() => sut.Remove(index);
            
            // Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        //** Remove() **//
        [Test]
        public void Remove_OnDisposedObject_ThrowsObjectDisposedException([Random(0, 256, 1)] int i)
        {
            // Arrange
            var sut = GetSut();
            var c = (char) i;
            sut.Dispose();

            // Act
            void CallOnDisposedObject() => sut.Remove(0, c);
            
            // Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task Remove_OtherCharacters_DoesNotAffect(int count)
        {
            // Arrange
            using var sut = GetSut();
            const int startIndex = 1;
            IList<char> chars = new[] {'t', 'e', 's', 't'}.ToList();
            foreach (var c in chars)
                await sut.AppendAsync(c);
            for (var i = 0; i < count; i++)
                chars.RemoveAt(startIndex);
            
            // Act
            sut.Remove(startIndex, count);
            
            // Assert
            for (var i = 0; i < sut.Length; i++)
            {
                var actual = await sut.GetAsCharAsync(i);
                var expected = chars.ElementAt(i);
                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public async Task Remove_TotalOfIndexAndCountIsHigherThanLength_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            using var sut = GetSut();
            const char ch1 = 't', ch2 = 'e';
            await sut.AppendAsync(ch1);
            await sut.AppendAsync(ch2);
            var index = sut.Length - 1;
            var count = sut.Length;

            // Act
            void SendBadParameters() => sut.Remove(index, count);
            
            // Assert
            Assert.That(SendBadParameters, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task Remove_WhenRemoved_DecreasesLength(int count)
        {
            // Arrange
            using var sut = GetSut();
            const char ch1 = 't', ch2 = 'e';
            await sut.AppendAsync(ch1);
            await sut.AppendAsync(ch2);
            var expected = sut.Length - count;
            
            // Act
            sut.Remove(0, count);
            var actual = sut.Length;
            
            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task ShallowClone_AppendingToObject_ChangesCloned([Random(0, 256, 1)] int i1, [Random(0, 256, 1)] int i2)
        {
            // Arrange
            using var sut = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2;
            await sut.AppendAsync(ch1);
            
            // Act
            var clone = sut.ShallowClone();
            await sut.AppendAsync(ch2);
            var actual = clone.Length;
            var expected = sut.Length;
            
            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task ShallowClone_ClonedObjectsToSafeBytes_ReturnsEqualSafeBytes([Random(0, 256, 1)] int i1,
            [Random(0, 256, 1)] int i2, [Random(0, 256, 1)] int i3)
        {
            // Arrange
            using var sut = GetSut();
            char ch1 = (char) i1, ch2 = (char) i2, ch3 = (char) i3;
            await sut.AppendAsync(ch1);
            await sut.AppendAsync(ch2);
            await sut.AppendAsync(ch3);
            var expected = await (await sut.ToSafeBytesAsync()).ToByteArrayAsync();
            
            // Act
            var clone = sut.ShallowClone();
            var actual = await  (await clone.ToSafeBytesAsync()).ToByteArrayAsync();
            var areEqual = expected.SequenceEqual(actual);
            
            // Assert
            Assert.That(areEqual, Is.True);
        }

        [Test]
        public async Task ShallowClone_ClonedObjectsValue_IsEqual([Random(0, 256, 1)] int i1)
        {
            // Arrange
            using var sut = GetSut();
            var expected = (char) i1;
            await sut.AppendAsync(expected);
            var clone = sut.ShallowClone();

            // Act
            var actual = await clone.GetAsCharAsync(0);
            
            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task ShallowClone_OnDisposedObject_ThrowsObjectDisposedException([Random(0, 256, 1)] int i1)
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync((char) i1);
            
            // Act
            sut.Dispose();
            void CallOnDisposedObject() => sut.ShallowClone();

            //Assert
            Assert.That(CallOnDisposedObject, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public async Task ToSafeBytesAsync_ChangingReturnedResult_OriginalObject()
        {
            // Arrange
            using var sut = GetSut();
            const char expected = 'a';
            const int index = 0;
            await sut.AppendAsync(expected);

            // Act
            var bytes = await sut.ToSafeBytesAsync();
            await bytes.AppendAsync(5);
            var actual = await sut.GetAsCharAsync(index);

            // Assert
            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public async Task ToSafeBytesAsync_DisposingReturnedResult_DoesNotAffectOriginalObject()
        {
            // Arrange
            using var sut = GetSut();
            const char expected = 'a';
            const int index = 0;
            await sut.AppendAsync(expected);

            // Act
            var bytes = await sut.ToSafeBytesAsync();
            bytes.Dispose();
            var actual = await sut.GetAsCharAsync(index);
            
            // Assert
            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public async Task ToSafeBytesAsync_ForMultipleChars_ReturnsEqualBytes()
        {
            // Arrange
            using var sut = GetSut();
            await sut.AppendAsync('a');
            await sut.AppendAsync('b');
            var charBytes1 = await sut.GetAsSafeBytes(0).ToByteArrayAsync();
            var charBytes2 = await sut.GetAsSafeBytes(1).ToByteArrayAsync();
            var expected = charBytes1.Combine(charBytes2);

            // Act
            var safeBytes = await sut.ToSafeBytesAsync();
            var actual = await safeBytes.ToByteArrayAsync();

            // Assert
            var areSame = expected.SequenceEqual(actual);
            Assert.That(areSame, Is.True);
        }

        [Test]
        public async Task ToSafeBytesAsync_ForSingleChar_ReturnsEqualBytes()
        {
            // Arrange
            using var sut = GetSut();
            await sut.AppendAsync('a');
            var expected = await sut.GetAsSafeBytes(0).ToByteArrayAsync();

            // Act
            var safeBytes = await sut.ToSafeBytesAsync();
            var actual = await safeBytes.ToByteArrayAsync();

            // Assert
            var areSame = expected.SequenceEqual(actual);
            Assert.That(areSame, Is.True);
        }

        [Test]
        public async Task ToSafeBytesAsync_OnDisposedObject_ThrowsObjectDisposedException()
        {
            //Arrange
            var sut = GetSut();
            await sut.AppendAsync('a');
            
            //Act
            sut.Dispose();
            Task CallOnDisposedObject() => sut.ToSafeBytesAsync();

            //Assert
            Assert.ThrowsAsync<ObjectDisposedException>(CallOnDisposedObject);
        }

        [Test]
        public void ToSafeBytesAsync_OnEmptyInstance_ThrowsInvalidOperationException()
        {
            // Arrange
            using var sut = GetSut();

            // Act
            Task CallOnEmptyObject() => sut.ToSafeBytesAsync();

            // Assert
            Assert.ThrowsAsync<InvalidOperationException>(CallOnEmptyObject);
        }
    }
}