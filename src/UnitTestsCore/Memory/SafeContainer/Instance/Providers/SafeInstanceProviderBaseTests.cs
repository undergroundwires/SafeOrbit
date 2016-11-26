
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using Moq;
using NUnit.Framework;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Providers
{
    /// <seealso cref="SafeInstanceProviderBase{TestInstanceProvider}"/>
    /// <seealso cref="IInstanceProvider"/>
    [TestFixture]
    public class SafeInstanceProviderBaseTests
    {
        [Test, TestCaseSource(typeof(InstanceCases), nameof(InstanceCases.InstanceProtectionModes))]
        public void Constructor_Sets_ProtectionMode(InstanceProtectionMode mode)
        {
            //arrange
            var expected = mode;
            //act
            var sut = GetSut(protectionMode: expected);
            var actual = sut.CurrentProtectionMode;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(InstanceCases), nameof(InstanceCases.LifeTimes))]
        public void Constructor_Sets_LifeTime(LifeTime lifeTime)
        {
            //arrange
            var expected = lifeTime;
            //act
            var sut = GetSut(lifeTime: expected);
            var actual = sut.LifeTime;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(InstanceCases), nameof(InstanceCases.ProtectionModeAndProtectVariables))]
        public void Constructor_Sets_Inner_InjectionDetectorSettings_From_ProtectionMode(
            InstanceProtectionMode protectionMode, bool scanState, bool scanCode)
        {
            //arrange
            var injectionDetector = Mock.Of<IInjectionDetector>();
            injectionDetector.ScanCode = !scanCode;
            injectionDetector.ScanState = !scanState;
            //act
            var sut = GetSut(protectionMode: protectionMode, injectionDetector: injectionDetector);
            //assert
            Assert.That(injectionDetector.ScanState, Is.EqualTo(scanState));
            Assert.That(injectionDetector.ScanCode, Is.EqualTo(scanCode));
        }

        [Test, TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
        public void Constructor_Sets_AlertChannel(InjectionAlertChannel alertChannel)
        {
            //arrange
            var expected = alertChannel;
            //act
            var sut = GetSut(alertChannel: expected);
            var actual = sut.AlertChannel;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))
        ]
        public void AlertChannel_Sets_InnerInjectionDetectorChannel(InjectionAlertChannel alertChannel)
        {
            //arrange
            var expected = alertChannel;
            var injectionDetector = new Mock<IInjectionDetector>();
            var sut = GetSut(injectionDetector: injectionDetector.Object);
            //act
            sut.AlertChannel = expected;
            //assert
            injectionDetector.VerifySet(
                detector => detector.AlertChannel = expected
            );
        }

        [Test, TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))
        ]
        public void AlertChannel_Gets_InnerInjectionDetectorChannel(InjectionAlertChannel alertChannel)
        {
            //arrange
            var expected = alertChannel;
            var injectionDetector = new Mock<IInjectionDetector>();
            var sut = GetSut(injectionDetector: injectionDetector.Object);
            //act
            var temp = sut.AlertChannel;
            //assert
            injectionDetector.VerifyGet(
                detector => detector.AlertChannel);
        }

        [Test]
        public void CanAlert_ForNonProtectedMode_returnsFalse()
        {
            const bool expected = false;
            var nonProtectedMode = InstanceProtectionMode.NoProtection;
            var sut = GetSut(protectionMode: nonProtectedMode);
            var actual = sut.CanAlert;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(InstanceCases), nameof(InstanceCases.AlertingInstanceProtectionModes))]
        public void CanAlert_ForProtectModes_returnsTrue(InstanceProtectionMode protectionMode)
        {
            const bool expected = true;
            var sut = GetSut(protectionMode: protectionMode);
            var actual = sut.CanAlert;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(InstanceCases), nameof(InstanceCases.ProtectionModeAndProtectVariables))]
        public void SetProtectionMode_Sets_Inner_InjectionDetectorSettings(InstanceProtectionMode protectionMode,
            bool scanState, bool scanCode)
        {
            //arrange
            var injectionDetector = Mock.Of<IInjectionDetector>();
            injectionDetector.ScanCode = !scanCode;
            injectionDetector.ScanState = !scanState;
            //act
            var sut = GetSut(protectionMode: protectionMode, injectionDetector: injectionDetector);
            sut.SetProtectionMode(protectionMode);
            //assert
            Assert.That(injectionDetector.ScanState, Is.EqualTo(scanState));
            Assert.That(injectionDetector.ScanCode, Is.EqualTo(scanCode));
        }

        [Test, TestCaseSource(typeof(InstanceCases), nameof(InstanceCases.NonStateProtection_To_StateProtection))]
        public void SetProtectionMode_From_NonStateProtection_To_StateProtection_NotifiesInnerInjectionDetector(InstanceProtectionMode from, InstanceProtectionMode to)
        {
            //arrange
            var detectorMock = new Mock<IInjectionDetector>();
            detectorMock.SetupProperty(d => d.ScanState);
            var sut = GetSut(protectionMode: from,
                injectionDetector: detectorMock.Object);
            sut.Provide(); //first call to the state
            detectorMock.ResetCalls();
            //act
            sut.SetProtectionMode(to);
            var temp = sut.Provide(); //will trigger lazy logic
            //assert
            detectorMock.Verify(d => d.NotifyChanges(It.IsAny<object>()), Times.Once);
        }

        [Test, TestCaseSource(typeof(InstanceCases), nameof(InstanceCases.StateProtection_To_NonStateProtection))]
        public void SetProtectionMode_From_StateProtection_To_NonStateProtection_DoesNotNotifyInnerInjectionDetector(InstanceProtectionMode from, InstanceProtectionMode to)
        {
            //arrange
            var detectorMock = new Mock<IInjectionDetector>();
            var sut = GetSut(protectionMode: from,
                injectionDetector: detectorMock.Object);
            detectorMock.ResetCalls();
            //act
            sut.SetProtectionMode(to);
            //assert
            detectorMock.Verify(d => d.NotifyChanges(It.IsAny<object>()), Times.Never);
        }

        private static SafeInstanceProviderBase<string> GetSut(LifeTime lifeTime = LifeTime.Unknown,
            InstanceProtectionMode protectionMode = InstanceProtectionMode.NoProtection,
            IInjectionDetector injectionDetector = null,
            InjectionAlertChannel alertChannel = InjectionAlertChannel.DebugFail)
        {
            return new TestInstanceProvider(
                alertChannel: alertChannel,
                injectionDetector: injectionDetector ?? Stubs.Get<IInjectionDetector>(),
                lifeTime: lifeTime,
                protectionMode: protectionMode);
        }

        private class TestInstanceProvider : SafeInstanceProviderBase<string>
        {
            public TestInstanceProvider(LifeTime lifeTime, IInjectionDetector injectionDetector,
                InstanceProtectionMode protectionMode, InjectionAlertChannel alertChannel) : base(
                lifeTime, injectionDetector, protectionMode, alertChannel)
            {
            }

            public override string GetInstance() => "aq";
        }
    }
}