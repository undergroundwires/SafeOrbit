using NUnit.Framework;
using SafeOrbit.Library;
using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Memory.InjectionServices.Alerters
{
    /// <seealso cref="RaiseEventAlerter" />
    /// <seealso cref="IAlerter" />
    [TestFixture]
    internal class RaiseEventAlerterTests : AlerterTestsBase<RaiseEventAlerter>
    {
        protected override RaiseEventAlerter GetSut() => new RaiseEventAlerter();
        public override InjectionAlertChannel CoveredChannel { get; } = InjectionAlertChannel.RaiseEvent;
        public override void Alert_Sut_Alerts_Message(TestDelegate alertingMessage, IInjectionMessage message)
        {
            object sender = null;
            IInjectionMessage args = null;
            LibraryManagement.LibraryInjected += (s, e) =>
            {
                sender = s;
                args = e;
            };
            alertingMessage.Invoke();
            Assert.That(args, Is.EqualTo(message));
            Assert.That(sender, Is.EqualTo(message.InjectedObject));
        }
    }
}