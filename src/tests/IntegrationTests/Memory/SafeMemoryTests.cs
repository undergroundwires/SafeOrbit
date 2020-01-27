using NUnit.Framework;
using SafeOrbit.Extensions;

namespace SafeOrbit.Memory
{
    /// <seealso cref="SafeMemoryStream" />
    [TestFixture]
    public class SafeMemoryTests
    {
        [Test]
        public void Can_write_read_and_close()
        {
            Assert.DoesNotThrow(() =>
            {
                var expected = new byte[] { 5, 10, 15, 20, 25, 30 };
                using var sut = new SafeMemoryStream();
                var buffer = expected.CopyToNewArray();
                sut.Write(buffer, 0, buffer.Length);
                var part1 = new byte[buffer.Length];
                sut.Read(part1, 0, 3);
            });
        }
    }
}