using System.Collections.ObjectModel;
using NUnit.Framework;
using SafeOrbit.Extensions;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory
{
    [TestFixture]
    public class SafeMemoryStreamTests : TestsFor<SafeMemoryStream>
    {
        [Test]
        public void ReadByte_ForAFewBytes_ReadsNextByteAndAdvances()
        {
            // Arrange
            var expected = new byte[]{1,2,3,4,5};
            var sut = GetSut();
            sut.Write(expected.CopyToNewArray(), 0, expected.Length);

            // Act
            var actual = new Collection<byte>();
            for (var i = 0; i < expected.Length; i++)
            {
                var result = sut.ReadByte();
                actual.Add((byte)result);
            }

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ReadByte_ForAFewBytes_ReturnsMinusOneOnEndOfSteam()
        {
            // Arrange
            var bytes = new byte[3];
            var sut = GetSut();
            sut.Write(bytes.CopyToNewArray(), 0, bytes.Length);

            // Act
            for (var i = 0; i < bytes.Length; i++)
                sut.ReadByte();
            var eofResult = sut.ReadByte();

            // Assert
            Assert.AreEqual(eofResult, -1);
        }

        protected override SafeMemoryStream GetSut()
            => new SafeMemoryStream();
    }
}
