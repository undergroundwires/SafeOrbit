using NUnit.Framework;

namespace SafeOrbit.Library.Build
{
    /// <see cref="IBuildInfo" />
    /// <see cref="BuildInfo" />
    [TestFixture]
    public class BuildInfoTests
    {
        [Test]
        public void TargetPlatform_Is_Expected()
        {
            var expected = GetExpectedPlatform();
            var sut = new BuildInfo();
            var actual = sut.TargetPlatform;
            Assert.That(actual, Is.EqualTo(expected));
        }

        private static Platform GetExpectedPlatform()
        {
#if NET45
            return Platform.Net450;
#elif NETCOREAPP1_1
            return Platform.NetStandard1_6;
#elif NETCOREAPP2_1 || NETCOREAPP2_2 || NETCOREAPP3_0 || NETCOREAPP3_1
            return Platform.NetStandard2;
#endif
        }
    }
}