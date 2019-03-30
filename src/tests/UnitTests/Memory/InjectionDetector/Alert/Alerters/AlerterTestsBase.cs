using System;
using System.Collections.Generic;
using NUnit.Framework;
using SafeOrbit.Memory.Injection;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Memory.InjectionServices.Alerters;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.InjectionServices.Alerters
{
    [TestFixture]
    internal abstract class AlerterTestsBase<T> : TestsFor<T> where T: class, IAlerter
    {
        public abstract InjectionAlertChannel ExpectedChannel { get; }

        [Test]
        public void Channel_Is_CoveredChannel()
        {
            var expected = ExpectedChannel;
            var sut = GetSut();
            var actual = sut.Channel;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(Cases), nameof(Cases.MessageCases))]
        public void Asserts_The_Message(IInjectionMessage message)
        {
            var sut = GetSut();
            var action = new TestDelegate(() => sut.Alert(message));
            Alert_Sut_Alerts_Message(action, message);
        }

        public abstract void Alert_Sut_Alerts_Message(TestDelegate alertingMessage, IInjectionMessage message);
    }

    public class Cases
    {
        
        public static IEnumerable<IInjectionMessage> MessageCases
    {
        get
        {
            yield return new InjectionMessage(true, true, 5);
            yield return new InjectionMessage(true, false, "aq");
            yield return new InjectionMessage(false, true, new byte[] { 5, 10, 15 });
        }
    }
}
}