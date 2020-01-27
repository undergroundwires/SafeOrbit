using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SafeOrbit.Memory.Injection;
using SafeOrbit.Memory.InjectionServices.Alerters;

namespace SafeOrbit.Memory.InjectionServices
{
    /// <seealso cref="InjectionAlerter" />
    /// <seealso cref="IInjectionAlerter" />
    /// <seealso cref="InjectionAlertChannel" />
    [TestFixture]
    public class InjectionAlerterTests
    {
        [Test]
        [TestCaseSource(typeof(InjectionAlerterTests), nameof(MessageCases))]
        public void Alert_ChannelIsRaiseEvent_InvokesRaiseEventAlerter(IInjectionMessage info)
        {
            //arrange
            const InjectionAlertChannel channel = InjectionAlertChannel.RaiseEvent;
            var alerterMock = new Mock<IAlerter>();
            var sut = GetSut(raiseEventAlerter: alerterMock.Object);
            //act
            sut.Alert(info, channel);
            //assert
            alerterMock.Verify(a => a.Alert(It.Is<IInjectionMessage>(value => info.Equals(value))));
        }

        [Test]
        [TestCaseSource(typeof(InjectionAlerterTests), nameof(MessageCases))]
        public void Alert_ChannelIsDebugFail_InvokesRaiseEventAlerter(IInjectionMessage info)
        {
            //arrange
            const InjectionAlertChannel channel = InjectionAlertChannel.DebugFail;
            var alerterMock = new Mock<IAlerter>();
            var sut = GetSut(debugFailAlerter: alerterMock.Object);
            //act
            sut.Alert(info, channel);
            //assert
            alerterMock.Verify(a => a.Alert(It.Is<IInjectionMessage>(value => info.Equals(value))));
        }

        [Test]
        [TestCaseSource(typeof(InjectionAlerterTests), nameof(MessageCases))]
        public void Alert_ChannelIsThrowException_InvokesRaiseEventAlerter(IInjectionMessage info)
        {
            //arrange
            const InjectionAlertChannel channel = InjectionAlertChannel.ThrowException;
            var alerterMock = new Mock<IAlerter>();
            var sut = GetSut(throwExceptionAlerter: alerterMock.Object);
            //act
            sut.Alert(info, channel);
            //assert
            alerterMock.Verify(a => a.Alert(It.Is<IInjectionMessage>(value => info.Equals(value))));
        }

        [Test]
        [TestCaseSource(typeof(InjectionAlerterTests), nameof(MessageCases))]
        public void Alert_ChannelIsDebugWrite_InvokesRaiseEventAlerter(IInjectionMessage info)
        {
            //arrange
            const InjectionAlertChannel channel = InjectionAlertChannel.DebugWrite;
            var alerterMock = new Mock<IAlerter>();
            var sut = GetSut(debugWriteAlerter: alerterMock.Object);
            //act
            sut.Alert(info, channel);
            //assert
            alerterMock.Verify(a => a.Alert(It.Is<IInjectionMessage>(value => info.Equals(value))));
        }

        private static IInjectionAlerter GetSut(
            IAlerter raiseEventAlerter = null,
            IAlerter debugWriteAlerter = null,
            IAlerter debugFailAlerter = null,
            IAlerter throwExceptionAlerter = null)
        {
            var mock = new Mock<IAlerterFactory>();
            //Mock the factory
            if (raiseEventAlerter != null || debugWriteAlerter != null || debugFailAlerter != null ||
                throwExceptionAlerter != null)
            {
                if (raiseEventAlerter != null)
                    mock.Setup(m => m.Get(InjectionAlertChannel.RaiseEvent)).Returns(raiseEventAlerter);
                if (debugWriteAlerter != null)
                    mock.Setup(m => m.Get(InjectionAlertChannel.DebugWrite)).Returns(debugWriteAlerter);
                if (debugFailAlerter != null)
                    mock.Setup(m => m.Get(InjectionAlertChannel.DebugFail)).Returns(debugFailAlerter);
                if (throwExceptionAlerter != null)
                    mock.Setup(m => m.Get(InjectionAlertChannel.ThrowException)).Returns(throwExceptionAlerter);
            }

            return new InjectionAlerter(mock.Object);
        }

        private static IEnumerable<IInjectionMessage> MessageCases
        {
            get
            {
                yield return new InjectionMessage(false, true, 5);
                yield return new InjectionMessage(true, true, "aq");
                yield return new InjectionMessage(true, false, DateTime.UtcNow);
            }
        }
    }
}