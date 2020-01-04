using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.SafeBytesServices.Collection
{
    /// <seealso cref="ByteIdListSerializer" />
    /// >
    [TestFixture]
    internal class ByteIdListSerializerTests : TestsFor<IByteIdListSerializer<int>>
    {
        protected override IByteIdListSerializer<int> GetSut() => new ByteIdListSerializer();

        [Test]
        public void DeserializeAsync_NullArgument_ThrowsArgumentNullException()
        {
            // arrange
            var sut = GetSut();
            var argument = (byte[]) null;

            // act
            async Task CallWithNullArgument() => await sut.DeserializeAsync(argument);
            // assert
            Assert.ThrowsAsync<ArgumentNullException>(CallWithNullArgument);
        }

        [Test]
        public void SerializeAsync_NullArgument_ThrowsArgumentNullException()
        {
            // arrange
            var sut = GetSut();
            var argument = (IReadOnlyCollection<int>) null;

            // act
            async Task CallWithNullArgument() => await sut.SerializeAsync(argument);
            // assert
            Assert.ThrowsAsync<ArgumentNullException>(CallWithNullArgument);
        }

        [Test]
        public async Task DeserializeAsync_EmptyListSerialized_CanDeserialize()
        {
            // arrange
            var sut = GetSut();
            var expected = new int[0];
            // act
            var serialized = await sut.SerializeAsync(expected);
            var actual = await sut.DeserializeAsync(serialized);
            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task DeserializeAsync_SingleIntSerialized_CanDeserialize()
        {
            // arrange
            var sut = GetSut();
            var expected = new[] {5};
            // act
            var serialized = await sut.SerializeAsync(expected);
            var actual = await sut.DeserializeAsync(serialized);
            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task DeserializeAsync_MultipleIntsSerialized_CanDeserialize()
        {
            // arrange
            var sut = GetSut();
            var expected = new[] {5, 50, 30, 120, 5, 50, 30, 120, 70};
            // act
            var serialized = await sut.SerializeAsync(expected);
            var actual = await sut.DeserializeAsync(serialized);
            // assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(-1)]
        [TestCase(1)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public async Task DeserializeAsync_BoundariesAreSerialized_CanDeserialize(
            int boundary)
        {
            // arrange
            var sut = GetSut();
            var expected = new[] {5, boundary, 30};
            // act
            var serialized = await sut.SerializeAsync(expected);
            var actual = await sut.DeserializeAsync(serialized);
            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}