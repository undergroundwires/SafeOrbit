using NUnit.Framework;
using SafeOrbit.Exceptions;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory.Injection
{
    /// <seealso cref="IInjectionDetector"/>
    /// <seealso cref="InjectionDetector"/>
    [TestFixture]
    public class InjectionDetectorTests
    {
        [Test]
        public void Alerts_OnStateInjection()
        {
            var obj = new Lines {Line1 = "1", Line2 = "2", Line3 = "3"};
            var sut = GetSut(scanCode: true, scanState: false);
            sut.AlertChannel = InjectionAlertChannel.ThrowException;
            sut.NotifyChanges(obj); //register
            obj.Line2 = "3"; //change but do not notify
            var alert = new TestDelegate(() => sut.AlertUnnotifiedChanges(obj));
            Assert.That(alert, Throws.TypeOf<MemoryInjectionException>());
        }

        [Test]
        public void Alerts_OnCodeInjection()
        {
            var obj = new Lines { Line1 = "1", Line2 = "2", Line3 = "3" };
            var obj2 = GetSut(scanCode: false, scanState: true);
            var sut = GetSut(false,false);
            sut.AlertChannel = InjectionAlertChannel.ThrowException;
            sut.NotifyChanges(obj); //register
            var alert = new TestDelegate(() => sut.AlertUnnotifiedChanges(obj2));
            Assert.That(alert, Throws.TypeOf<MemoryInjectionException>());
        }

        private class Lines
        {
            public string Line1 { get; set; }
            public string Line2 { get; set; }
            public string Line3 { get; set; }
        }
        private static IInjectionDetector GetSut(bool scanState, bool scanCode)
            => new InjectionDetector(scanState: scanState, scanCode: scanCode);
    }
}