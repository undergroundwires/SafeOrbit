using System;
using Moq;
using NUnit.Framework;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Memory.InjectionServices.Reflection;
using SafeOrbit.Memory.InjectionServices.Stampers;
using SafeOrbit.Tests.Cases;

namespace SafeOrbit.Memory.Injection
{
    /// <seealso cref="IInjectionDetector" />
    /// <seealso cref="InjectionDetector" />
    /// <seealso cref="InjectionDetectorFaker" />
    [TestFixture]
    internal class InjectionDetectorTests
    {
        [Test]
        public void Constructor_AllProtectionVariables_areTrue()
        {
            // Arrange
            var sut = GetSut();
            const bool expectedStateProtection = true;
            const bool expectedCodeProtection = true;

            // Act
            var actualStateProtection = sut.ScanState;
            var actualCodeProtection = sut.ScanCode;

            // Assert
            Assert.That(actualStateProtection, Is.EqualTo(expectedStateProtection));
            Assert.That(actualCodeProtection, Is.EqualTo(expectedCodeProtection));
        }

        [Test]
        [TestCaseSource(typeof(CommonCases), nameof(CommonCases.TrueFalse))]
        public void Constructor_Sets_JustState(bool expected)
        {
            // Arrange
            var sut = GetSut(protectState: expected);

            // Act
            var actual = sut.ScanState;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(CommonCases), nameof(CommonCases.TrueFalse))]
        public void Constructor_Sets_JustCode(bool expected)
        {
            // Arrange
            var sut = GetSut(protectCode: expected);

            // Act
            var actual = sut.ScanCode;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
        public void Constructor_Sets_InjectionAlertChannel(InjectionAlertChannel expected)
        {
            // Arrange
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(
                protectState: true, protectCode: true,
                isStateValid: false, isCodeValid: false,
                alerter: alerterMock.Object,
                channel: expected);
            var testObject = new object();
            sut.AlertChannel = expected;

            // Act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);

            // Assert
            alerterMock.Verify(alerter =>
                alerter.Alert(
                    It.IsAny<IInjectionMessage>(),
                    It.Is<InjectionAlertChannel>(m => m.Equals(expected))));
        }

        [Test]
        public void NotifyChanges_WhenObjectParameterIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var sut = GetSut();
            var nullArgument = (object) null;

            // Act
            void InvokingWithNullArgument() => sut.NotifyChanges(nullArgument);

            // Assert
            Assert.That(InvokingWithNullArgument, Throws.ArgumentNullException);
        }

        [Test]
        public void NotifyChanges_JustStateIsTrue_InvokesStateStamper()
        {
            // Arrange
            const int testObject = 5;
            const bool protectState = true;
            var stateStamperMock = new Mock<IStamper<object>>();
            var sut = GetSut(stateStamper: stateStamperMock.Object, protectState: protectState);
            
            // Act
            sut.NotifyChanges(testObject);
           
            // Assert
            stateStamperMock.Verify(stamper => stamper.GetStamp(testObject), Times.Once);
        }

        [Test]
        public void NotifyChanges_ProtectStateIsFalse_DoesNotInvokeStateStamper()
        {
            // Arrange
            const int testObject = 5;
            const bool protectState = false;
            var stateStamperMock = new Mock<IStamper<object>>();
            var sut = GetSut(stateStamper: stateStamperMock.Object,
                protectState: protectState);
            
            // Act
            sut.NotifyChanges(testObject);
          
            // Assert
            stateStamperMock.Verify(stamper => stamper.GetStamp(It.IsAny<object>()), Times.Never);
        }

        [Test]
        public void NotifyChanges_ProtectCodeIsFalse_DoesNotInvokeCodeStamper()
        {
            // Arrange
            const int testObject = 5;
            var codeStamperMock = new Mock<IStamper<Type>>();
            var sut = GetSut(
                codeStamper: codeStamperMock.Object,
                protectCode: false);
           
            // Act
            sut.NotifyChanges(testObject);
           
            // Assert
            codeStamperMock.Verify(stamper => stamper.GetStamp(It.IsAny<Type>()), Times.Never);
        }

        [Test]
        public void AlertUnnotifiedChanges_WhenObjectParameterIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var sut = GetSut();
            var nullObject = (object) null;

            // Act
            void InvokingWithNullObject() => sut.AlertUnnotifiedChanges(nullObject);

            // Assert
            Assert.That(InvokingWithNullObject, Throws.ArgumentNullException);
        }

        [Test]
        public void AlertUnnotifiedChanges_WithOnlyStateProtection_ForSameStamps_DoesNotAlert()
        {
            // Arrange
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(isStateValid: true, isCodeValid: true,
                alerter: alerterMock.Object);
            var testObject = new object();

            // Act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);

            // Assert
            alerterMock.Verify(alerter =>
                    alerter.Alert(
                        It.IsAny<IInjectionMessage>(),
                        It.IsAny<InjectionAlertChannel>()),
                Times.Never);
        }

        [Test]
        public void AlertUnnotifiedChanges_WithOnlyCodeProtection_ForSameStamps_DoesNotAlert()
        {
            // Arrange
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(
                protectState: false, protectCode: true,
                isStateValid: true, isCodeValid: true,
                alerter: alerterMock.Object);
            var testObject = new object();

            // Act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);

            // Assert
            alerterMock.Verify(alerter =>
                    alerter.Alert(
                        It.IsAny<IInjectionMessage>(),
                        It.IsAny<InjectionAlertChannel>()),
                Times.Never);
        }

        [Test]
        public void AlertUnnotifiedChanges_WithOnlyStateProtection_ForDifferentCodeStamps_DoesNotAlert()
        {
            // Arrange
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(
                protectState: true, protectCode: false,
                isStateValid: true, isCodeValid: false,
                alerter: alerterMock.Object);
            var testObject = new object();
            
            // Act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            
            // Assert
            alerterMock.Verify(alerter =>
                    alerter.Alert(
                        It.IsAny<IInjectionMessage>(),
                        It.IsAny<InjectionAlertChannel>()),
                Times.Never);
        }

        [Test]
        public void AlertUnnotifiedChanges_WithOnlyCodeProtection_ForDifferentStateStamps_DoesNotAlert()
        {
            // Arrange
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(
                protectState: false, protectCode: true,
                isStateValid: false, isCodeValid: true,
                alerter: alerterMock.Object);
            var testObject = new object();
            
            // Act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            
            // Assert
            alerterMock.Verify(alerter =>
                    alerter.Alert(
                        It.IsAny<IInjectionMessage>(),
                        It.IsAny<InjectionAlertChannel>()),
                Times.Never);
        }

        [Test]
        public void AlertUnnotifiedChanges_WithAllProtections_ForSameStamps_DoesNotAlert()
        {
            // Arrange
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(
                protectState: true, protectCode: true,
                isStateValid: true, isCodeValid: true,
                alerter: alerterMock.Object);
            var testObject = new object();
            
            // Act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            
            // Assert
            alerterMock.Verify(alerter =>
                    alerter.Alert(
                        It.IsAny<IInjectionMessage>(),
                        It.IsAny<InjectionAlertChannel>()),
                Times.Never);
        }

        [Test]
        public void AlertUnnotifiedChanges_WithAllProtections_ForDifferentStateStamps_Alerts()
        {
            // Arrange
            const InjectionType expectedInjectionType = InjectionType.VariableInjection;
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(
                protectState: true, protectCode: true,
                isStateValid: false, isCodeValid: true,
                alerter: alerterMock.Object);
            var testObject = new object();
            
            // Act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            
            // Assert
            alerterMock.Verify(alerter =>
                    alerter.Alert(
                        It.Is<IInjectionMessage>(m => m.InjectionType == expectedInjectionType),
                        It.IsAny<InjectionAlertChannel>()),
                Times.Once);
        }

        [Test]
        public void AlertUnnotifiedChanges_WithAllProtections_ForDifferentCodeStamps_Alerts()
        {
            // Arrange
            var alerterMock = new Mock<IInjectionAlerter>();
            const InjectionType expectedInjectionType = InjectionType.CodeInjection;
            var sut = GetSut(isStateValid: true, isCodeValid: false,
                alerter: alerterMock.Object);
            var testObject = new object();
            
            // Act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            
            // Assert
            alerterMock.Verify(alerter =>
                    alerter.Alert(
                        It.Is<IInjectionMessage>(m => m.InjectionType == expectedInjectionType),
                        It.IsAny<InjectionAlertChannel>()),
                Times.Once);
        }

        [Test]
        public void AlertUnnotifiedChanges_WithAllProtections_ForDifferentCodeAndStateStamps_Alerts()
        {
            // Arrange
            const InjectionType expectedInjectionType = InjectionType.CodeAndVariableInjection;
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(isStateValid: false, isCodeValid: false,
                alerter: alerterMock.Object);
            var testObject = new object();
            
            // Act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            
            // Assert
            alerterMock.Verify(alerter =>
                    alerter.Alert(
                        It.Is<IInjectionMessage>(m => m.InjectionType == expectedInjectionType),
                        It.IsAny<InjectionAlertChannel>()),
                Times.Once);
        }

        [Test]
        public void AlertUnnotifiedChanges_WithOnlyStateProtection_ForDifferentStateStamps_Alerts()
        {
            // Arrange
            const InjectionType expectedInjectionType = InjectionType.VariableInjection;
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(isStateValid: false, isCodeValid: true,
                alerter: alerterMock.Object);
            var testObject = new object();
            
            // Act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            
            // Assert
            alerterMock.Verify(alerter =>
                    alerter.Alert(
                        It.Is<IInjectionMessage>(m => m.InjectionType == expectedInjectionType),
                        It.IsAny<InjectionAlertChannel>()),
                Times.Once);
        }

        [Test]
        public void AlertUnnotifiedChanges_WithOnlyCodeProtection_ForDifferentCodeStamps_Alerts()
        {
            // Arrange
            const InjectionType expectedInjectionType = InjectionType.CodeInjection;
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(isStateValid: true, isCodeValid: false,
                alerter: alerterMock.Object);
            var obj = new object();
            
            // Act
            sut.NotifyChanges(obj);
            sut.AlertUnnotifiedChanges(obj);
            
            // Assert
            alerterMock.Verify(alerter =>
                    alerter.Alert(
                        It.Is<IInjectionMessage>(m => m.InjectionType == expectedInjectionType),
                        It.IsAny<InjectionAlertChannel>()),
                Times.Once);
        }


        [Test]
        [TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
        public void InjectionAlertChannel_WhenAlerting_UsedForInternalAlerter(InjectionAlertChannel expected)
        {
            // Arrange
            var alerterMock = new Mock<IInjectionAlerter>();
            var sut = GetSut(
                protectState: true, protectCode: true,
                isStateValid: false, isCodeValid: false,
                alerter: alerterMock.Object);
            var testObject = new object();
            sut.AlertChannel = expected;
            
            // Act
            sut.NotifyChanges(testObject);
            sut.AlertUnnotifiedChanges(testObject);
            
            // Assert
            alerterMock.Verify(alerter =>
                alerter.Alert(
                    It.IsAny<IInjectionMessage>(),
                    It.Is<InjectionAlertChannel>(m => m.Equals(expected))));
        }

        [Test]
        [TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.CanAlertCases))]
        public bool CanAlert_WithDifferentProtectionSettings_ReturnsVector(bool protectCode, bool protectState)
        {
            var sut = GetSut(protectState: protectState, protectCode: protectCode);
            return sut.CanAlert;
        }

        private static IInjectionDetector GetSut(bool protectCode = true, bool protectState = true,
            bool isStateValid = true, bool isCodeValid = true, IStamper<object> stateStamper = null,
            IStamper<Type> codeStamper = null, IInjectionAlerter alerter = null,
            InjectionAlertChannel channel = Defaults.AlertChannel)
        {
            var codeStampMock = new Mock<IStamp>();
            codeStampMock.Setup(m => m.Equals(It.IsAny<IStamp<int>>())).Returns(isCodeValid);
            var stateStampMock = new Mock<IStamp>();
            stateStampMock.Setup(m => m.Equals(It.IsAny<IStamp<int>>())).Returns(isStateValid);
            var stateStamperMock = new Mock<IStamper<object>>();
            stateStamperMock.Setup(m => m.GetStamp(It.IsAny<object>())).Returns(stateStampMock.Object);
            var codeStamperMock = new Mock<IStamper<Type>>();
            codeStamperMock.Setup(m => m.GetStamp(It.IsAny<Type>())).Returns(codeStampMock.Object);
            return new InjectionDetector(alerter ?? Mock.Of<IInjectionAlerter>(), Stubs.Get<ITypeIdGenerator>(),
                stateStamper ?? stateStamperMock.Object, codeStamper ?? codeStamperMock.Object, protectCode,
                protectState, channel);
        }
    }
}