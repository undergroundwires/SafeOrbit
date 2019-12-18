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

        [Test]
        public void Equals_TwoStringsWithDifferentLength_ReturnsFalse()
        {
            // arrange
            var sut1 = new SafeString();
            sut1.Append("hej");
            var sut2 = new SafeString();
            sut2.Append("sp");
            // act
            var equality = sut1.Equals(sut2);
            var equalityOpposite = sut2.Equals(sut1);
            // assert
            Assert.False(equality);
            Assert.False(equalityOpposite);
        }
    }
}