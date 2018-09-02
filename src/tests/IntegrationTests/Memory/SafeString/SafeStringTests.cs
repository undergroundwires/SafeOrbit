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
    }
}