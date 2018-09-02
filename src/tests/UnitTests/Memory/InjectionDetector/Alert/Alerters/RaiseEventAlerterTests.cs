using System;
using Moq;
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
        private readonly Mock<SafeOrbitCore> _safeOrbitCoreMock = new Mock<SafeOrbitCore>();
        protected override RaiseEventAlerter GetSut() => new RaiseEventAlerter(_safeOrbitCoreMock.Object);
        public override InjectionAlertChannel ExpectedChannel { get; } = InjectionAlertChannel.RaiseEvent;
        public override void Alert_Sut_Alerts_Message(TestDelegate alertingMessage, IInjectionMessage message)
        {
            _safeOrbitCoreMock.ResetCalls();
            alertingMessage.Invoke();
            _safeOrbitCoreMock.Verify(core=> core.AlertInjection(message.InjectedObject, message));
        }
    }
}