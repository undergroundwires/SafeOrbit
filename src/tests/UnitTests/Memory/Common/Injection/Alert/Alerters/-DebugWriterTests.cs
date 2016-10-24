using NUnit.Framework;
using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Memory.InjectionServices.Alerters
{
    /// <seealso cref="DebugWriteAlerter" />
    /// <seealso cref="IAlerter" />
    [TestFixture]
    internal class DebugWriterTests : AlerterTestsBase<DebugWriteAlerter>
    {
        protected override DebugWriteAlerter GetSut() => new DebugWriteAlerter();
        public override InjectionAlertChannel CoveredChannel { get; } = InjectionAlertChannel.DebugWrite;
        public override void Alert_Sut_Alerts_Message(TestDelegate alertingMessage, IInjectionMessage message)
        {
            base.IgnoreTest();
        }
    }
}