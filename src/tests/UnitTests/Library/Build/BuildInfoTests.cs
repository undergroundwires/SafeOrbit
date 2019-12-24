using NUnit.Framework;

namespace SafeOrbit.Library.Build
{
    /// <see cref="IBuildInfo"/>
    /// <see cref="BuildInfo"/>
    [TestFixture]
    public class BuildInfoTests
    {
        [Test]
        public void TargetPlatform_Is_NetStandard()
        {
            const Platform expected = Platform.NetStandard2;
            var sut = new BuildInfo();
            var actual = sut.TargetPlatform;
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}