using NUnit.Framework;
using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Memory.InjectionServices.Alerters
{
    /// <seealso cref="DebugFailAlerter" />
    /// <seealso cref="IAlerter" />
    [TestFixture]
    internal class DebugFailAlerterTests : AlerterTestsBase<DebugFailAlerter>
    {
        protected override DebugFailAlerter GetSut() => new DebugFailAlerter();

        public override InjectionAlertChannel ExpectedChannel { get; } = InjectionAlertChannel.DebugFail;

        public override void Alert_Sut_Alerts_Message(TestDelegate alertingMessage, IInjectionMessage message)
        {
            IgnoreTest();
        }
    }
}