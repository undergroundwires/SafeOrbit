using NUnit.Framework;
using SafeOrbit.Exceptions;
using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Memory.InjectionServices.Alerters
{
    /// <seealso cref="ThrowExceptionAlerter" />
    /// <seealso cref="IAlerter" />
    [TestFixture]
    internal class ThrowExceptionAlerterTests : AlerterTestsBase<ThrowExceptionAlerter>
    {
        protected override ThrowExceptionAlerter GetSut() => new ThrowExceptionAlerter();
        public override InjectionAlertChannel ExpectedChannel { get; } = InjectionAlertChannel.ThrowException;

        public override void Alert_Sut_Alerts_Message(TestDelegate alertingMessage, IInjectionMessage message)
        {
            Assert.That(alertingMessage,
                Throws.TypeOf<MemoryInjectionException>()
                    .And.With.Property(nameof(MemoryInjectionException.InjectionType)).EqualTo(message.InjectionType)
                    .And.With.Property(nameof(MemoryInjectionException.DetectionTime))
                    .EqualTo(message.InjectionDetectionTime)
                    .And.With.Property(nameof(MemoryInjectionException.InjectedObject)).EqualTo(message.InjectedObject)
            );
        }
    }
}