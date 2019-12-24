using System.Linq;
using NUnit.Framework;
using SafeOrbit.Extensions;
using Shared.Extensions;

namespace SafeOrbit.Memory
{
    /// <seealso cref="SafeMemoryStream"/>
    [TestFixture]
    public class SafeMemoryStreamTests
    {
        [Test]
        public void Write_Clears_Input_Buffer()
        {
            using var sut = GetSut();
            var buffer = new byte[] {5, 10, 15, 20, 25, 30};
            sut.Write(buffer,0, buffer.Length);
            Assert.That(buffer, Is.Empty.Or.All.EqualTo(0));
        }

        [Test]
        public void Read_Clears_Inner_Buffer()
        {
            //arrange
            var expected1 = new byte[] { 5, 10, 15 };
            var expected2 = new byte[] {20, 25, 30};
            using var sut = GetSut();
            var buffer = expected1.Combine(expected2);
            sut.Write(buffer, 0, buffer.Length);
            //act
            var part1 = new byte[3];
            sut.Read(part1, 0, 3);
            var part2 = new byte[3];
            sut.Read(part2, 0, 3);
            //assert
            Assert.That(part1, Is.EqualTo(expected1));
            Assert.That(part2, Is.EqualTo(expected2));
            Assert.That(sut.Length, Is.Zero);
        }

        [Test]
        public void Write_AfterBeingRead_CanWrite()
        {
            //arrange
            var expected1 = new byte[] { 5, 10, 15 };
            var expected2 = new byte[] { 20, 25, 30 };
            using var sut = GetSut();
            var buffer = expected1.CopyToNewArray();
            sut.Write(buffer, 0, buffer.Length);
            var actual1 = new byte[expected1.Length];
            sut.Read(actual1, 0, 3);
            // Act
            buffer = expected2.CopyToNewArray();
            sut.Write(buffer, 0, buffer.Length);
            var actual2 = new byte[expected2.Length];
            sut.Read(actual2, 0, 3);
            // Assert
            Assert.That(actual1, Is.EqualTo(expected1));
            Assert.That(actual2, Is.EqualTo(expected2));
        }

        [Test]
        public void CanWrite_InstanceIsClosed_returnsFalse()
        {
            var sut = GetSut();
            sut.Close();
            Assert.False(sut.CanWrite);
        }

        private static SafeMemoryStream GetSut() => new SafeMemoryStream();
    }
}