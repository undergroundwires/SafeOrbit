
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

using System;
using Moq;
using NUnit.Framework;
using SafeOrbit.Common.Reflection;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Memory.InjectionServices.Stampers;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Memory.Injection
{
    /// <seealso cref="IInjectionDetector"/>
    /// <seealso cref="InjectionDetector"/>
    /// <seealso cref="InjectionDetectorFaker"/>
    [TestFixture]
    internal class InjectionDetectorTests
    {
        [Test]
        public void Constructor_AllProtectionVariables_areTrue()
        {
            var sut = GetSut();
            const bool expectedStateProtection = true;
            const bool expectedCodeProtection = true;
            var actualStateProtection = sut.ScanState;
            var actualCodeProtection = sut.ScanCode;
            Assert.That(actualStateProtection, Is.EqualTo(expectedStateProtection));
            Assert.That(actualCodeProtection, Is.EqualTo(expectedCodeProtection));
        }
        [Test, TestCaseSource(typeof(CommonCases), nameof(CommonCases.TrueFalse))]
        public void Constructor_Sets_JustState(bool expected)
        {
            var sut = GetSut(protectState: expected);
            var actual = sut.ScanState;
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test, TestCaseSource(typeof(CommonCases), nameof(CommonCases.TrueFalse))]
        public void Constructor_Sets_JustCode(bool expected)
        {
            var sut = GetSut(protectCode: expected);
            var actual = sut.ScanCode;
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test, TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
        public void Constructor_Sets_InjectionAlertChannel(InjectionAlertChannel expected)
        {
            //arrange
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(
                protectState: true, protectCode: true,
                isStateValid: false, isCodeValid: false,
                alerter: alerterMock.Object,
                channel: expected);
            var testObject = new object();
            sut.AlertChannel = expected;
            //act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            //assert
            alerterMock.Verify((alerter) =>
            alerter.Alert(
                It.IsAny<IInjectionMessage>(),
                It.Is<InjectionAlertChannel>((m) => m.Equals(expected))));
        }

        [Test]
        public void NotifyChanges_WhenObjectParameterIsNull_throwsArgumentNullException()
        {
            var sut = GetSut();
            var nullArgument = (object) null;
            TestDelegate invokingWithNullArgument = () => sut.NotifyChanges(nullArgument);
            Assert.That(invokingWithNullArgument, Throws.ArgumentNullException);
        }
        [Test]
        public void NotifyChanges_JustStateIsTrue_invokesStateStamper()
        {
            //arrange
            const int testObject = 5;
            const bool protectState = true;
            var stateStamperMock = new Mock<IStamper<object>>();
            var sut = GetSut(stateStamper: stateStamperMock.Object, protectState: protectState);
            //act
            sut.NotifyChanges(testObject);
            //assert
            stateStamperMock.Verify(stamper => stamper.GetStamp(testObject), Times.Once);
        }
        [Test]
        public void NotifyChanges_ProtectStateIsFalse_doesNotInvokeStateStamper()
        {
            //arrange
            const int testObject = 5;
            const bool protectState = false;
            var stateStamperMock = new Mock<IStamper<object>>();
            var sut = GetSut(stateStamper: stateStamperMock.Object,
                protectState: protectState);
            //act
            sut.NotifyChanges(testObject);
            //assert
            stateStamperMock.Verify(stamper => stamper.GetStamp(It.IsAny<object>()), Times.Never);
        }
        [Test]
        public void NotifyChanges_ProtectStateIsFalse_doesNotInvokeCodeStamper()
        {
            //arrange
            const int testObject = 5;
            const bool JustCode = false;
            var codeStamperMock = new Mock<IStamper<Type>>();
            var sut = GetSut(codeStamper: codeStamperMock.Object);
            //act
            sut.NotifyChanges(testObject);
            //assert
            codeStamperMock.Verify(stamper => stamper.GetStamp(It.IsAny<Type>()), Times.Never);
        }
        [Test]
        public void AlertUnnotifiedChanges_WhenObjectParameterIsNull_throwsArgumentNullException()
        {
            var sut = GetSut();
            var nullObject = (object)null;
            TestDelegate invokingWithNullObject = () => sut.AlertUnnotifiedChanges(nullObject);
            Assert.That(invokingWithNullObject, Throws.ArgumentNullException);
        }
        [Test]
        public void AlertUnnotifiedChanges_WhenObjectParameterIsNull_throwsArgumentNullException()
        {
            var sut = GetSut();
            var nullObject = (object)null;
            TestDelegate invokingWithNullObject = () => sut.AlertUnnotifiedChanges(nullObject);
            Assert.That(invokingWithNullObject, Throws.ArgumentNullException);
        }
        [Test]
        public void AlertUnnotifiedChanges_WithOnlyStateProtection_ForSameStamps__doesNotAlert()
        {
            //arrange
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(isStateValid: true, isCodeValid: true,
                alerter: alerterMock.Object);
            var testObject = new object();
            //act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            //assert
            alerterMock.Verify((alerter) =>
            alerter.Alert(
                It.IsAny<IInjectionMessage>(),
                It.IsAny<InjectionAlertChannel>()),
                Times.Never);
        }
        [Test]
        public void AlertUnnotifiedChanges_WithOnlyCodeProtection_ForSameStamps_doesNotAlert()
        {
            //arrange
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(
                protectState: false, protectCode: true,
                isStateValid: true, isCodeValid: true,
                alerter: alerterMock.Object);
            var testObject = new object();
            //act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            //assert
            alerterMock.Verify((alerter) =>
            alerter.Alert(
                It.IsAny<IInjectionMessage>(),
                It.IsAny<InjectionAlertChannel>()),
                Times.Never);
        }
        [Test]
        public void AlertUnnotifiedChanges_WithOnlyStateProtection_ForDifferentCodeStamps_doesNotAlert()
        {
            //arrange
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(
                protectState: true, protectCode: false,
                isStateValid: true, isCodeValid: false,
                alerter: alerterMock.Object);
            var testObject = new object();
            //act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            //assert
            alerterMock.Verify((alerter) =>
            alerter.Alert(
                It.IsAny<IInjectionMessage>(),
                It.IsAny<InjectionAlertChannel>()),
                Times.Never);
        }
        [Test]
        public void AlertUnnotifiedChanges_WithOnlyCodeProtection_ForDifferentStateStamps_doesNotAlert()
        {
            //arrange
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(
                protectState: false, protectCode: true,
                isStateValid: false, isCodeValid: true,
                alerter: alerterMock.Object);
            var testObject = new object();
            //act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            //assert
            alerterMock.Verify((alerter) =>
            alerter.Alert(
                It.IsAny<IInjectionMessage>(),
                It.IsAny<InjectionAlertChannel>()),
                Times.Never);
        }
        [Test]
        public void AlertUnnotifiedChanges_WithAllProtections_ForSameStamps_doesNotAlert()
        {
            //arrange
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(
                protectState: true, protectCode: true,
                isStateValid: true, isCodeValid: true,
                alerter: alerterMock.Object);
            var testObject = new object();
            //act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            //assert
            alerterMock.Verify((alerter) =>
            alerter.Alert(
                It.IsAny<IInjectionMessage>(),
                It.IsAny<InjectionAlertChannel>()),
                Times.Never);
        }
        [Test]
        public void AlertUnnotifiedChanges_WithAllProtections_ForDifferentStateStamps_alerts()
        {
            //arrange
            const InjectionType expectedInjectionType = InjectionType.VariableInjection;
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(
                protectState: true, protectCode: true,
                isStateValid: false, isCodeValid: true,
                alerter: alerterMock.Object);
            var testObject = new object();
            //act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            //assert
            alerterMock.Verify((alerter) =>
            alerter.Alert(
                It.Is<IInjectionMessage>(m=>m.InjectionType == expectedInjectionType),
                It.IsAny<InjectionAlertChannel>()),
                Times.Once);
        }
        [Test]
        public void AlertUnnotifiedChanges_WithAllProtections_ForDifferentCodeStamps_alerts()
        {
            //arrange
            var alerterMock = new Mock<IInjectionAlerter>();
            const InjectionType expectedInjectionType = InjectionType.CodeInjection;
            var sut = GetSut(isStateValid: true, isCodeValid: false,
                alerter: alerterMock.Object);
            var testObject = new object();
            //act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            //assert
            alerterMock.Verify((alerter) =>
              alerter.Alert(
                  It.Is<IInjectionMessage>(m => m.InjectionType == expectedInjectionType),
                  It.IsAny<InjectionAlertChannel>()),
                  Times.Once);
        }
        [Test]
        public void AlertUnnotifiedChanges_WithAllProtections_ForDifferentCodeAndStateStamps_alerts()
        {
            //arrange
            var expectedInjectionType = InjectionType.CodeAndVariableInjection;
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(isStateValid: false, isCodeValid: false,
                alerter: alerterMock.Object);
            var testObject = new object();
            //act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            //assert
            alerterMock.Verify((alerter) =>
            alerter.Alert(
                It.Is<IInjectionMessage>(m => m.InjectionType == expectedInjectionType),
                It.IsAny<InjectionAlertChannel>()),
                Times.Once);
        }
        [Test]
        public void AlertUnnotifiedChanges_WithOnlyStateProtection_ForDifferentStateStamps_alerts()
        {
            //arrange
            const InjectionType expectedInjectionType = InjectionType.VariableInjection;
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(isStateValid: false, isCodeValid: true,
                alerter: alerterMock.Object);
            var testObject = new object();
            //act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            //assert
            alerterMock.Verify((alerter) =>
            alerter.Alert(
                It.Is<IInjectionMessage>(m => m.InjectionType == expectedInjectionType),
                It.IsAny<InjectionAlertChannel>()),
                Times.Once);
        }
        [Test]
        public void AlertUnnotifiedChanges_WithOnlyCodeProtection_ForDifferentCodeStamps_alerts()
        {
            //arrange
            const InjectionType expectedInjectionType = InjectionType.CodeInjection;
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(isStateValid: true, isCodeValid: false,
                alerter: alerterMock.Object);
            var obj = new object();
            //act
            sut.NotifyChanges(obj);
            sut.AlertUnnotifiedChanges(obj);
            //assert
            alerterMock.Verify((alerter) =>
            alerter.Alert(
                It.Is<IInjectionMessage>(m => m.InjectionType == expectedInjectionType),
                It.IsAny<InjectionAlertChannel>()),
                Times.Once);
        }


        [Test, TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
        public void InjectionAlertChannel_WhenAlerting_UsedForInternalAlerter(InjectionAlertChannel expected)
        {
            //arrange
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(
                protectState: true, protectCode: true,
                isStateValid: false, isCodeValid: false,
                alerter: alerterMock.Object);
            var testObject = new object();
            sut.AlertChannel = expected;
            //act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            //assert
            alerterMock.Verify((alerter) =>
            alerter.Alert(
                It.IsAny<IInjectionMessage>(),
                It.Is<InjectionAlertChannel>((m) => m.Equals(expected))));
        }

        [Test, TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.CanAlertCases))]
        public bool CanAlert_WithDifferentProtectionSettings_returnsVector(bool protectCode, bool protectState)
        {
            var sut = GetSut(protectState: protectState, protectCode: protectCode);
            return sut.CanAlert;
        }

        private static IInjectionDetector GetSut(bool protectCode = true, bool protectState = true, bool isStateValid = true, bool isCodeValid = true, IStamper<object> stateStamper = null, IStamper<Type> codeStamper = null, IInjectionAlerter alerter = null, InjectionAlertChannel channel = Defaults.AlertChannel)
        {
            var codeStampMock = new Mock<IStamp>();
            codeStampMock.Setup(m => m.Equals(It.IsAny<IStamp<int>>())).Returns(isCodeValid);
            var stateStampMock = new Mock<IStamp>();
            stateStampMock.Setup(m => m.Equals(It.IsAny<IStamp<int>>())).Returns(isStateValid);
            var stateStamperMock = new Mock<IStamper<object>>();
            stateStamperMock.Setup((m) => m.GetStamp(It.IsAny<object>())).Returns(stateStampMock.Object);
            var codeStamperMock = new Mock<IStamper<Type>>();
            codeStamperMock.Setup((m) => m.GetStamp(It.IsAny<Type>())).Returns(codeStampMock.Object);
            return new InjectionDetector(alerter ?? Mock.Of<IInjectionAlerter>(), Stubs.Get<ITypeIdGenerator>(), stateStamper ?? stateStamperMock.Object, codeStamper ?? codeStamperMock.Object, protectCode, protectState, channel);
        }
    }
}