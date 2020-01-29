using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Cryptography.Encryption;
using SafeOrbit.Cryptography.Random;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.SafeBytesServices.DataProtection;
using SafeOrbit.Memory.SafeBytesServices.Factory;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.SafeBytesServices.Collection
{
    /// <seealso cref="ISafeByteCollection" />
    /// <seealso cref="EncryptedSafeByteCollection" />
    [TestFixture]
    internal partial class EncryptedSafeByteCollectionTests : TestsFor<ISafeByteCollection>
    {
        private static Task<ISafeByte> GetSafeByteAsync(byte b)
        {
            var safeByteFactory = Stubs.Get<ISafeByteFactory>();
            return safeByteFactory.GetByByteAsync(b);
        }

        protected override ISafeByteCollection GetSut()
        {
            return new EncryptedSafeByteCollection(
                encryptor: Stubs.Get<IFastEncryptor>(),
                encryptionKey: Stubs.Get<IMemoryProtectedBytes>(),
                fastRandom: Stubs.Get<IFastRandom>(),
                safeByteFactory: Stubs.Get<ISafeByteFactory>(),
                serializer: Stubs.Get<IByteIdListSerializer<int>>());
        }


        [Test]
        public void AppendAsync_ForDisposedObject_throwsObjectDisposedException([Random(0, 256, 1)] byte b)
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();

            // Act
            async Task AppendByte() => await sut.AppendAsync(await GetSafeByteAsync(b));

            // Assert
            Assert.That(AppendByte, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public async Task AppendAsync_GetAsync_SafeBytes_AppendsSingle_CanGet()
        {
            // Arrange
            var sut = GetSut();
            const byte expected = 55;
            await sut.AppendAsync(await GetSafeByteAsync(expected));

            // Act
            var actual = await (await sut.GetAsync(0)).RevealDecryptedByteAsync();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task AppendAsync_GetAsync_SafeBytes_AppendsAtTheEnd_CanGet()
        {
            // Arrange
            var sut = GetSut();
            const byte expectedAt1 = 69, expectedAt2 = 13;
            await sut.AppendAsync(await GetSafeByteAsync(31));
            await sut.AppendAsync(await GetSafeByteAsync(expectedAt1));
            await sut.AppendAsync(await GetSafeByteAsync(expectedAt2));

            // Act
            var actualAt1 = await (await sut.GetAsync(1)).RevealDecryptedByteAsync();
            var actualAt2 = await (await sut.GetAsync(2)).RevealDecryptedByteAsync();

            // Assert
            Assert.AreEqual(expectedAt1, actualAt1);
            Assert.AreEqual(expectedAt2, actualAt2);
        }

        [Test]
        public void AppendAsync_ArgumentIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var sut = GetSut();
            ISafeByte nullInstance = null;

            // Act
            Task AppendNull() => sut.AppendAsync(nullInstance);

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(AppendNull);
        }

        [Test]
        public void AppendManyAsync_ForEmptyList_DoesNotThrow()
        {
            // Arrange
            var sut = GetSut();
            var list = Enumerable.Empty<ISafeByte>();

            // Act
            Task AppendNull() => sut.AppendManyAsync(list);

            // Assert
            Assert.DoesNotThrowAsync(AppendNull);
        }

        [Test]
        public async Task AppendManyAsync_NonEmptyList_IncreasesLength()
        {
            // Arrange
            var sut = GetSut();
            var list = new[] { await GetSafeByteAsync(0), await GetSafeByteAsync(1), await GetSafeByteAsync(2) };
            var expected = list.Length;

            // Act
            await sut.AppendManyAsync(list);
            var actual = sut.Length;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task AppendManyAsync_AppendedList_CanBeRetrieved()
        {
            // Arrange
            var sut = GetSut();
            var list = new[] { await GetSafeByteAsync(0), await GetSafeByteAsync(1), await GetSafeByteAsync(2) };
            var expectedIds = list.Select(l => l.Id).ToArray();

            // Act
            await sut.AppendManyAsync(list);

            var actualIds = new int[3];
            actualIds[0] = (await sut.GetAsync(0)).Id;
            actualIds[1] = (await sut.GetAsync(1)).Id;
            actualIds[2] = (await sut.GetAsync(2)).Id;

            // Assert
            CollectionAssert.AreEqual(expectedIds, actualIds);
        }

        [Test]
        public void AppendManyAsync_DisposedObject_ThrowsObjectDisposedException()
        {
            // Arrange
            var sut = GetSut();
            sut.Dispose();

            // Assert
            async Task AppendByte() => await sut.AppendManyAsync(new[] { await GetSafeByteAsync(5) });

            // Act
            Assert.ThrowsAsync<ObjectDisposedException>(AppendByte);
        }

        //** Dispose **//
        [Test]
        public async Task Dispose_OnAnInstanceWithBytes_SetsLengthToZero([Random(0, 256, 1)] byte b1,
            [Random(0, 256, 1)] byte b2, [Random(0, 256, 1)] byte b3)
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync(await GetSafeByteAsync(b1));
            await sut.AppendAsync(await GetSafeByteAsync(b2));
            await sut.AppendAsync(await GetSafeByteAsync(b3));

            // Act
            sut.Dispose();
            var actual = sut.Length;

            // Assert
            Assert.That(actual, Is.Zero);
        }

        //** Get **//
        [Test]
        public async Task GetAsync_OnDisposedObject_ThrowsObjectDisposedException([Random(0, 256, 1)] byte b)
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync(await GetSafeByteAsync(b));
            const int position = 0;
            
            // Act
            sut.Dispose();
            async Task CallOnDisposedObject() => await sut.GetAsync(position);

            // Assert
            Assert.ThrowsAsync<ObjectDisposedException>(CallOnDisposedObject);
        }

        [Test]
        public void GetAsync_OnEmptyInstance_ThrowsInvalidOperationException()
        {
            // Arrange
            var sut = GetSut();
            const int position = 0;

            // Act
            async Task CallOnEmptyInstance() => await sut.GetAsync(position);
            
            // Assert
            Assert.ThrowsAsync<InvalidOperationException>(CallOnEmptyInstance);
        }

        [Test]
        public async Task GetAsync_WhenRequestedPositionIsLowerThanRange_ThrowsArgumentOutOfRangeException(
            [Random(0, 256, 1)] byte b)
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync(await GetSafeByteAsync(b));
            const int position = -1;

            // Act
            async Task CallWithBadParameter() => await sut.GetAsync(position);

            // Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(CallWithBadParameter);
        }

        [Test]
        public async Task GetAsync_WhenRequestedPositionIsHigherThanRange_ThrowsArgumentOutOfRangeException(
            [Random(0, 256, 1)] byte b)
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync(await GetSafeByteAsync(b));
            const int position = 1;

            // Act
            async Task CallWithBadParameter() => await sut.GetAsync(position);
            
            // Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(CallWithBadParameter);
        }

        [Test]
        public async Task GetAsync_ForMultipleAppendedBytes_ReturnsTheBytes()
        {
            // Arrange
            var sut = GetSut();
            const byte byteAt0 = 0, byteAt1 = 1, byteAt2 = 2;
            await sut.AppendAsync(await GetSafeByteAsync(byteAt0));
            await sut.AppendAsync(await GetSafeByteAsync(byteAt1));
            await sut.AppendAsync(await GetSafeByteAsync(byteAt2));

            // Act
            var actualAt0 = await (await sut.GetAsync(0)).RevealDecryptedByteAsync();
            var actualAt1 = await (await sut.GetAsync(1)).RevealDecryptedByteAsync();
            var actualAt2 = await (await sut.GetAsync(2)).RevealDecryptedByteAsync();

            // Assert
            Assert.AreEqual(byteAt0, actualAt0);
            Assert.AreEqual(byteAt1, actualAt1);
            Assert.AreEqual(byteAt2, actualAt2);
        }

        [Test]
        public async Task GetAsync_ForSingleAppendedByte_ReturnsByte()
        {
            // Arrange
            var sut = GetSut();
            var safeByte = await GetSafeByteAsync(5);

            // Act
            await sut.AppendAsync(safeByte);
            var safebyteBack = await sut.GetAsync(0);
            
            // Assert
            Assert.That(safeByte, Is.EqualTo(safebyteBack));
        }

        [Test]
        public async Task GetAsync_AfterModifications_ReturnsAsExpected()
        {
            // Arrange
            var expected = new[] { await GetSafeByteAsync(5), await GetSafeByteAsync(10)};
            var sut = GetSut();
            await sut.AppendAsync(expected[0]);
            
            // Act
            _ = await sut.GetAsync(0); // Calling once first to test the encryption/decryption harmony with Append
            await sut.AppendAsync(expected[1]);
            var actual = new[]
            {
                await sut.GetAsync(0),
                await sut.GetAsync(1)
            };
            
            // Assert
            Assert.True(expected[0].Equals(actual[0]));
            Assert.True(expected[1].Equals(actual[1]));
        }

        [Test]
        public async Task GetAsync_AfterCallingToDecryptedBytes_ReturnsAsExpected()
        {
            // Arrange
            var expected = await GetSafeByteAsync(5);
            var sut = GetSut();
            await sut.AppendAsync(expected);
            
            // Act
            _ = await sut.RevealDecryptedBytesAsync(); // Calling once first to test the encryption/decryption harmony with ToDecryptedBytes
            var actual = await sut.GetAsync(0);
            
            // Assert
            Console.Write($"Expected id {expected.Id}, actual id {actual.Id}");
            Assert.True(expected.Equals(actual));
        }


        [Test]
        public async Task GetAllAsync_OnDisposedObject_ThrowsObjectDisposedException()
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync(await GetSafeByteAsync(1));

            // Act
            sut.Dispose();
            async Task CallOnDisposedObject() => await sut.GetAllAsync();

            // Assert
            Assert.ThrowsAsync<ObjectDisposedException>(CallOnDisposedObject);
        }

        [Test]
        public async Task GetAsync_OnEmptyInstance_ReturnsEmptyList()
        {
            // Arrange
            var sut = GetSut();

            // Act
            var actual = await sut.GetAllAsync();

            // Assert
            Assert.NotNull(actual);
            Assert.IsEmpty(actual);
        }

        [Test]
        public async Task GetAllAsync_ForMultipleAppendedBytes_ReturnsTheBytes()
        {
            // Arrange
            var sut = GetSut();
            var expected = new []
            {
                await GetSafeByteAsync(0),
                await GetSafeByteAsync(1),
                await GetSafeByteAsync(2)
            };

            // Act
            await sut.AppendManyAsync(expected);
            var actual = await sut.GetAllAsync();

            // Assert
            CollectionAssert.AreEqual(
                expected.Select(b=> b.Id),
                actual.Select(b=> b.Id)
                );
        }

        [Test]
        public async Task GetAllAsync_ForSingleAppendedByte_ReturnsByte()
        {
            // Arrange
            var sut = GetSut();
            var expected = await GetSafeByteAsync(0);

            // Act
            await sut.AppendAsync(expected);
            var actual = (await sut.GetAllAsync()).SingleOrDefault();

            // Assert
            Assert.AreEqual(expected.Id, actual.Id);
        }

        [Test]
        public async Task GetAllAsync_AfterModifications_ReturnsAsExpected()
        {
            // Arrange
            var expected = new[] { await GetSafeByteAsync(5), await GetSafeByteAsync(10) };
            var sut = GetSut();
            await sut.AppendAsync(expected[0]);

            // Act
            _ = await sut.GetAllAsync(); // Calling once first to test the encryption/decryption harmony with Append
            await sut.AppendAsync(expected[1]);
            var actual =await sut.GetAllAsync();

            // Assert
            Assert.True(expected[0].Equals(actual[0]));
            Assert.True(expected[1].Equals(actual[1]));
        }

        [Test]
        public async Task GetAllAsync_AfterCallingToDecryptedBytes_ReturnsAsExpected()
        {
            // Arrange
            var expected = await GetSafeByteAsync(5);
            var sut = GetSut();
            await sut.AppendAsync(expected);

            // Act
            await sut.RevealDecryptedBytesAsync(); // Calling once first to test the encryption/decryption harmony with ToDecryptedBytes
            var actual = (await sut.GetAllAsync()).SingleOrDefault();

            // Assert
            Console.Write($"Expected id {expected.Id}, actual id {actual.Id}");
            Assert.True(expected.Equals(actual));
        }

        [Test]
        public async Task Length_AfterSingleAppend_Increases([Random(0, 256, 1)] byte b)
        {
            // Arrange
            var sut = GetSut();
            const int expected = 1;
            await sut.AppendAsync(await GetSafeByteAsync(b));
            
            // Act
            var actual = sut.Length;
            
            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        //** Length **//
        [Test]
        public void Length_ForAFreshInstance_IsZero()
        {
            // Arrange
            var sut = GetSut();
            const int expected = 0;
            
            // Act
            var actual = sut.Length;
            
            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task Length_IncreasesAfterEachAppend_Increases()
        {
            // Arrange
            var sut = GetSut();
            const int expected1 = 0;
            const int expected2 = 1;
            const int expected3 = 2;
            const int expected4 = 3;
            
            // Act
            var actual1 = sut.Length;
            await sut.AppendAsync(await GetSafeByteAsync(1));
            var actual2 = sut.Length;
            await sut.AppendAsync(await GetSafeByteAsync(2));
            var actual3 = sut.Length;
            await sut.AppendAsync(await GetSafeByteAsync(3));
            var actual4 = sut.Length;
            
            // Assert
            Assert.That(actual1, Is.EqualTo(expected1));
            Assert.That(actual2, Is.EqualTo(expected2));
            Assert.That(actual3, Is.EqualTo(expected3));
            Assert.That(actual4, Is.EqualTo(expected4));
        }

        [Test]
        public async Task RevealDecryptedBytesAsync_ForInstanceHoldingMultipleBytes_returnsTheArrayOfBytes([Random(0, 256, 1)] byte b1,
            [Random(0, 256, 1)] byte b2, [Random(0, 256, 1)] byte b3)
        {
            // Arrange
            var bytes = new[] {b1, b2, b3};
            var sut = GetSut();
            await sut.AppendAsync(await GetSafeByteAsync(b1));
            await sut.AppendAsync(await GetSafeByteAsync(b2));
            await sut.AppendAsync(await GetSafeByteAsync(b3));
            
            // Act
            var byteArray = await sut.RevealDecryptedBytesAsync();
            
            // Assert
            CollectionAssert.AreEqual(byteArray, bytes);
        }

        [Test]
        public async Task RevealDecryptedBytesAsync_ForInstanceHoldingSingleByte_returnsTheArrayOfBytes([Random(0, 256, 1)] byte b)
        {
            // Arrange
            var bytes = new[] {b};
            var sut = GetSut();
            await sut.AppendAsync(await GetSafeByteAsync(b));
            
            // Act
            var byteArray = await sut.RevealDecryptedBytesAsync();
            
            // Assert
            CollectionAssert.AreEqual(byteArray, bytes);
        }

        [Test]
        public async Task RevealDecryptedBytesAsync_OnDisposedObject_throwsObjectDisposedException()
        {
            // Arrange
            var sut = GetSut();
            await sut.AppendAsync(await GetSafeByteAsync(5));
            
            // Act
            sut.Dispose();
            async Task CallingOnDisposedObject() => await sut.RevealDecryptedBytesAsync();

            // Assert
            Assert.ThrowsAsync<ObjectDisposedException>(CallingOnDisposedObject);
        }

        [Test]
        public void RevealDecryptedBytesAsync_OnEmptyInstance_throwsInvalidOperationException()
        {
            // Arrange
            var sut = GetSut();

            // Act
            Task CallingOnEmptyInstance() => sut.RevealDecryptedBytesAsync();
            
            // Assert
            Assert.ThrowsAsync<InvalidOperationException>(CallingOnEmptyInstance);

        }

        [Test]
        public async Task RevealDecryptedBytesAsync_ForMultipleBytes_returnsAsExpected()
        {
            // Arrange
            var expected = new byte[] {5, 10, 15};
            var sut = GetSut();
            foreach (var @byte in expected)
                await sut.AppendAsync(await GetSafeByteAsync(@byte));
            
            // Act
            var actual = await sut.RevealDecryptedBytesAsync();
            
            //Assert
            Assert.True(expected.SequenceEqual(actual));
        }

        [Test]
        public async Task RevealDecryptedBytesAsync_ForSingleByte_returnsAsExpected()
        {
            // Arrange
            var expected = new byte[] {5};
            var sut = GetSut();
            await sut.AppendAsync(await GetSafeByteAsync(5));

            // Act
            var actual = await sut.RevealDecryptedBytesAsync();

            // Assert
            Assert.True(expected.SequenceEqual(actual));
        }

        [Test]
        public async Task RevealDecryptedBytesAsync_CalledMultipleTimes_returnsExpected()
        {
            // Arrange
            var expected = new byte[] {5, 10, 15};
            var sut = GetSut();
            foreach (var @byte in expected)
                await sut.AppendAsync(await GetSafeByteAsync(@byte));

            // Act
            var actual = await sut.RevealDecryptedBytesAsync();
            var second = await sut.RevealDecryptedBytesAsync();

            //Assert
            Assert.True(expected.SequenceEqual(actual));
            Assert.True(expected.SequenceEqual(second));
        }

        [Test]
        public async Task RevealDecryptedBytesAsync_CalledAgainAfterModifications_returnsExpected()
        {
            // Arrange
            var raw = new byte[] {5, 10, 15};
            var sut = GetSut();
            foreach (var @byte in raw)
                await sut.AppendAsync(await GetSafeByteAsync(@byte));
            var modifications = new byte[] {10, 15, 30};
            
            // Act
            await sut.RevealDecryptedBytesAsync(); // Calling once first to test the encryption/decryption harmony with Append
            foreach (var @byte in modifications)
                await sut.AppendAsync(await GetSafeByteAsync(@byte));
            var afterModifications = await sut.RevealDecryptedBytesAsync();
            
            // Assert
            var expected = raw.Concat(modifications).ToArray();
            Console.WriteLine($"Expected: {string.Join(",", expected)}{Environment.NewLine}" +
                              $"Actual: {string.Join(",", afterModifications)}{Environment.NewLine}");
            Assert.True(afterModifications.SequenceEqual(expected));
        }
    }
}