using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.SafeBytesServices.Id;
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
        public void GetByte_returnsRightSafeByte(byte b)
        {
            //arrange
            var expected = b;
            var sut = _sut;
            //act
            var safeByte = sut.GetByByte(expected);
            var actual = safeByte.Get();
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(ByteCases), nameof(ByteCases.AllBytes))]
        public void GetInt_returnsRightSafeByte(byte b)
        {
            var sut = _sut;
            var expected = b;
            var byteId = Stubs.Get<IByteIdGenerator>().Generate(expected);
            var safeByte = sut.GetById(byteId);
            var actual = safeByte.Get();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetByte_WithoutInitialization_invokesInitializeMethod()
        {
            var mock = GetMock();
            var sut = mock.Object;
            try
            {
                sut.GetByByte(5);
            }
            catch
            {
                /*swallow exceptions*/
            }

            mock.Verify(f => f.Initialize(), Times.Once);
        }

        [Test]
        public void GetInt_WithoutInitialization_invokesInitializeMethod()
        {
            var mock = GetMock();
            var sut = mock.Object;
            try
            {
                sut.GetById(5);
            }
            catch
            {
                /*swallow exceptions*/
            }

            mock.Verify(f => f.Initialize(), Times.Once);
        }

        /// <summary>
        ///     Tests assert that we are efficient for many bytes.
        /// </summary>
        [Test]
        public void GetByBytes_MultipleBytes_ObjectGetterIsCalledOnce()
        {
            // Arrange
            var bytes = new byte[] { 1, 2, 3, 4, 5 };
            var stream = new SafeMemoryStream();
            stream.Write(bytes, 0, bytes.Length);

            const int byteId = 1;
            var hasherMock = new Mock<IByteIdGenerator>();
            hasherMock.Setup(b => b.GenerateMany(stream))
                .Returns(new []{ byteId, byteId, byteId, byteId, byteId });

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
            _ = sut.GetByBytes(stream).ToArray();

            // Assert
            objectMock.VerifyGet(o=>o.Object, Times.Once);
        }


        [Test]
        public void GetByBytes_NullStream_ThrowsException()
        {
            var sut = GetSut();
            void Act() => sut.GetByBytes(null).ToArray();
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Test]
        public void GetByBytes_ForEachIdFromGenerator_ReturnsId()
        {
            // Arrange
            var expected = new [] {10, 20, 30, 40, 50};
            var bytes = new byte[] { 1, 2, 3, 4, 5 };
            var stream = new SafeMemoryStream();
            stream.Write(bytes, 0, bytes.Length);

            var hasherMock = new Mock<IByteIdGenerator>();
            hasherMock.Setup(b => b.GenerateMany(stream))
                .Returns(expected);

            var sut = GetSut(byteIdGenerator: hasherMock.Object);

            // Act
            var actual = sut.GetByBytes(stream)
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