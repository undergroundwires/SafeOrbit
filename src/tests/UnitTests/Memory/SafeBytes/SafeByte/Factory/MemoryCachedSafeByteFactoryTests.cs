using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SafeOrbit.Extensions;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.SafeBytesServices.Id;
using SafeOrbit.Threading;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Memory.SafeBytesServices.Factory
{
    /// <seealso cref="ISafeByteFactory" />
    /// <seealso cref="MemoryCachedSafeByteFactory" />
    [TestFixture]
    internal class MemoryCachedSafeByteFactoryTests
    {
        private ISafeByteFactory _sut;

        [OneTimeSetUp]
        public void Init()
        {
            _sut = GetSut();
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public async Task GetByIdAsync_ExistingByteId_ReturnsRightSafeByte(byte b)
        {
            // Arrange
            var sut = _sut;
            var expected = b;
            var byteId = await Stubs.Get<IByteIdGenerator>().GenerateAsync(expected);

            // Act
            var safeByte = await sut.GetByIdAsync(byteId);

            // Assert
            var actual = await safeByte.GetAsync();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetByIdAsync_UnknownId_Throws()
        {
            // Arrange
            var sut = _sut;
            const int byteId = int.MinValue;

            // Act
            Task GetByUnknownId() => sut.GetByIdAsync(byteId);

            // Assert
            Assert.ThrowsAsync<KeyNotFoundException>(GetByUnknownId);
        }

        [Test]
        public async Task GetByIdAsync_WithoutInitialization_InvokesInitializeMethod()
        {
            // Arrange
            var mock = GetMock();
            var sut = mock.Object;

            // Act
            try
            {
                await sut.GetByIdAsync(5)
                    .ConfigureAwait(false);
            }
            catch { /*swallow exceptions*/ }

            // Assert
            mock.Verify(f => f.InitializeAsync(), Times.Once);
        }

        [Test]
        public void GetByIdsAsync_ArgumentNull_Throws()
        {
            // Arrange
            var sut = _sut;
            var ids = (int[]) null;

            // Act
            Task CallingWithNullArgument() => sut.GetByIdsAsync(ids);
            
            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(CallingWithNullArgument);
        }

        [Test]
        public async Task GetByIdsAsync_ArgumentEmpty_ReturnsEmptyList()
        {
            // Arrange
            var sut = _sut;
            var ids = Enumerable.Empty<int>();

            // Act
            var bytes = await sut.GetByIdsAsync(ids);

            // Assert
            Assert.NotNull(bytes);
            Assert.IsEmpty(bytes);
        }

        [Test]
        public void GetByIdAsync_ListHasUnknownId_Throws()
        {
            // Arrange
            var sut = _sut;
            var list = new [] {int.MinValue};

            // Act
            Task GetByUnknownId() => sut.GetByIdsAsync(list);

            // Assert
            Assert.ThrowsAsync<KeyNotFoundException>(GetByUnknownId);
        }

        [Test]
        public async Task GetByIdsAsync_ExistingIds_ReturnsRightBytes()
        {
            // Arrange
            var sut = _sut;
            var expected = new byte[] {1, 2, 3, 4, 5};
            var stream = new SafeMemoryStream(expected.CopyToNewArray());
            var byteIds = await Stubs.Get<IByteIdGenerator>().GenerateManyAsync(stream);

            // Act
            var safeBytes = await sut.GetByIdsAsync(byteIds);

            // Assert
            var actual = safeBytes.Select(safeByte => TaskContext.RunSync(safeByte.GetAsync)).ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public async Task GetByIdsAsync_WithoutInitialization_InvokesInitializeMethod()
        {
            // Arrange
            var mock = GetMock();
            var sut = mock.Object;

            // Act
            try
            {
                await sut.GetByIdsAsync(new []{5})
                    .ConfigureAwait(false);
            }
            catch { /*swallow exceptions*/ }

            // Assert
            mock.Verify(f => f.InitializeAsync(), Times.Once);
        }

        [Test]
        public async Task GetByByteAsync_WithoutInitialization_InvokesInitializeMethod()
        {
            // Arrange
            var mock = GetMock();
            var sut = mock.Object;

            // Act
            try
            {
                await sut.GetByByteAsync(5);
            }
            catch  {  /*swallow exceptions*/ }

            // Assert
            mock.Verify(f => f.InitializeAsync(), Times.Once);
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public async Task GetByByteAsync_Returns_Right_SafeByte(byte b)
        {
            // Arrange
            var expected = b;
            var sut = _sut;

            // Act
            var safeByte = await sut.GetByByteAsync(expected);

            // Assert
            var actual = await safeByte.GetAsync();
            Assert.That(actual, Is.EqualTo(expected));
        }


        /// <summary>
        ///     Tests assert that we are efficient for many bytes.
        /// </summary>
        [Test]
        public async Task GetByBytesAsync_MultipleBytes_ObjectGetterIsCalledOnce()
        {
            // Arrange
            var bytes = new byte[] { 1, 2, 3, 4, 5 };
            var stream = new SafeMemoryStream(bytes);

            const int byteId = 1;
            var hasherMock = new Mock<IByteIdGenerator>();
            hasherMock.Setup(b => b.GenerateManyAsync(stream))
                .ReturnsAsync(new []{ byteId, byteId, byteId, byteId, byteId });

            var objectMock = new Mock<ISafeObject<Dictionary<int, ISafeByte>>>();
            objectMock.Setup(o => o.Object)
                .Returns(new Dictionary<int, ISafeByte>
                {
                    {1, Stubs.Get<ISafeByte>()}
                });

            var factoryMock = new Mock<ISafeObjectFactory>();
            factoryMock.Setup(f => f.Get<Dictionary<int, ISafeByte>>(It.IsAny<IInitialSafeObjectSettings>()))
                .Returns(objectMock.Object);

            var sut = GetSut(factory: factoryMock.Object, byteIdGenerator: hasherMock.Object);

            // Act
            _ = (await sut.GetByBytesAsync(stream)).ToArray();

            // Assert
            objectMock.VerifyGet(o=>o.Object, Times.Once);
        }


        [Test]
        public void GetByBytesAsync_NullStream_ThrowsException()
        {
            // Arrange
            var sut = GetSut();

            // Act
            async Task Act() => (await sut.GetByBytesAsync(null)).ToArray();

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(Act);
        }

        [Test]
        public async Task GetByBytesAsync_WithoutInitialization_InvokesInitializeMethod()
        {
            // Arrange
            var mock = GetMock();
            var stream = new SafeMemoryStream(new byte[] { 1, 2, 3, 4, 5 });
            var sut = mock.Object;

            // Act
            try
            {
                await sut.GetByBytesAsync(stream);
            }
            catch {  /*swallow exceptions*/ }

            // Assert
            mock.Verify(f => f.InitializeAsync(), Times.Once);
        }

        [Test]
        public async Task GetByBytesAsync_ForEachIdFromGenerator_ReturnsId()
        {
            // Arrange
            var expected = new [] {10, 20, 30, 40, 50};
            var bytes = new byte[] { 1, 2, 3, 4, 5 };
            var stream = new SafeMemoryStream(bytes);

            var hasherMock = new Mock<IByteIdGenerator>();
            hasherMock.Setup(b => b.GenerateManyAsync(stream))
                .ReturnsAsync(expected);

            var sut = GetSut(byteIdGenerator: hasherMock.Object);

            // Act
            var actual = (await sut.GetByBytesAsync(stream))
                .Select(b=>b.Id)
                .ToArray();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        private static ISafeByteFactory GetSut(
            SafeObjectProtectionMode innerDictionaryProtectionMode =
                MemoryCachedSafeByteFactory.DefaultInnerDictionaryProtection,
            ISafeObjectFactory factory = null,
            IByteIdGenerator byteIdGenerator = null)
        {
            return new MemoryCachedSafeByteFactory(
                byteIdGenerator ?? Stubs.Get<IByteIdGenerator>(),
                Stubs.GetFactory<ISafeByte>(),
                factory ?? Stubs.Get<ISafeObjectFactory>(),
                innerDictionaryProtectionMode
            );
        }

        private static Mock<MemoryCachedSafeByteFactory> GetMock()
            =>
                new Mock<MemoryCachedSafeByteFactory>(
                    Stubs.Get<IByteIdGenerator>(),
                    Stubs.GetFactory<ISafeByte>(),
                    Stubs.Get<ISafeObjectFactory>(),
                    MemoryCachedSafeByteFactory.DefaultInnerDictionaryProtection) {CallBase = true};
    }
}