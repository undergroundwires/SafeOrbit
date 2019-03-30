using System.Threading;
using NUnit.Framework;

namespace SafeOrbit.Memory
{
    /// <seealso cref="SafeString" />
    /// <seealso cref="ISafeString" />
    [TestFixture] 
    public class SafeStringTests
    {
        [Test]
        public void SafeString_Returns_AppendedText()
        {
            var expected = "test";
            var sut = new SafeString();
            sut.Append("t");
            sut.Append("es");
            sut.Append('t');
            using (var sm = new SafeStringToStringMarshaler(sut))
            {
                var actual = sm.String;
                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public void SafeString_ModifiedByAnotherThread_ReturnsAppendedText()
        {
            var expected = "test";
            var sut = new SafeString();
            sut.Append("t");
            var thread = new Thread(() =>
            {
                sut.Append("es");
                sut.Append('t');
            });
            thread.Start();
            thread.Join();
            using (var sm = new SafeStringToStringMarshaler(sut))
            {
                var actual = sm.String;
                Assert.That(actual, Is.EqualTo(expected));
            }
        }
    }
}