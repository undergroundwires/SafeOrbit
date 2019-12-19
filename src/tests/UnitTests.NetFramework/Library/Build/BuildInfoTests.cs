using NUnit.Framework;

namespace SafeOrbit.Library.Build
{
    /// <see cref="IBuildInfo"/>
    /// <see cref="BuildInfo"/>
    [TestFixture]
    public class BuildInfoTests
    {
        [Test]
        public void TargetPlatform_Is_NetFramework()
        {
            var sut = new BuildInfo();
            var actual = sut.TargetPlatform;
            Assert.True(actual.ToString().StartsWith("Net4"));
        }
    }
}