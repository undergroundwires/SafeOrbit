using NUnit.Framework;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Library
{
    [TestFixture]
    public class SafeOrbitCoreTests
    {
        [Test]
        public void AlertChannel_AfterInitialized_CanSet()
        {
            // Arrange
            const InjectionAlertChannel expected = InjectionAlertChannel.DebugFail;
            var sut = SafeOrbitCore.Current;

            // Act
            sut.AlertChannel = expected;

            // Assert
            Assert.AreEqual(expected, sut.AlertChannel);
        }
    }
}
