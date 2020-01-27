using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using NUnit.Framework;
using SafeOrbit.Exceptions;
using SafeOrbit.Fakes;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Memory.InjectionServices.Protectable;

namespace SafeOrbit.Memory
{
    /// <seealso cref="SafeObject{TObject}" />
    /// <seealso cref="ISafeObject{TObject}" />
    /// <seealso cref="SafeObjectProtectionMode" />
    /// <seealso cref="IProtectable{TProtectionLevel}" />
    [TestFixture]
    public class SafeObjectTests : ProtectableBaseTests<SafeObjectProtectionMode>
    {
        [Test]
        [TestCaseSource(typeof(SafeObjectCases), nameof(SafeObjectCases.SafeObjectProtectionModeCases))]
        public void Constructor_Sets_ProtectionMode(SafeObjectProtectionMode mode)
        {
            // Arrange
            var expected = mode;

            // Act
            var sut = GetSut(protectionMode: expected);
            var actual = sut.CurrentProtectionMode;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_Sets_InitialObject()
        {
            var initialObject = new TestObject {TestProperty = "changed"};
            var sut = GetSut(initialObject);
            Assert.That(initialObject, Is.EqualTo(sut.Object));
        }

        [Test]
        [TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
        public void Constructor_Sets_AlertChannel(InjectionAlertChannel alertChannel)
        {
            // Arrange
            var expected = alertChannel;

            // Act
            var sut = GetSut(channel: expected);
            var actual = sut.AlertChannel;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Ctor_NoIsReadOnlyValue_DefaultsToFalse()
        {
            // Arrange
            var modifiableSut = GetSut();

            // Act
            var actual = modifiableSut.IsReadOnly;

            // Assert
            Assert.That(actual, Is.False);
        }


        [Test]
        public void Ctor_IsReadOnly_SetsToTrue()
        {
            // Arrange
            var nonModifiableSut = GetSut(isReadOnly: true);

            // Act
            var actual = nonModifiableSut.IsReadOnly;

            // Assert
            Assert.That(actual, Is.True);
        }

        [Test]
        public void Constructor_WithoutInitialObject_CreatesANewInstance()
        {
            // Arrange
            var sut = GetSut();

            // Act
            var newInstance = sut.Object;

            // Assert
            Assert.That(newInstance, Is.Not.Null);
            Assert.That(newInstance, Is.InstanceOf<TestObject>());
        }

        [Test]
        public void Constructor_InitialValueIsNotNull_NotifiesInnerInjectionDetector()
        {
            // Arrange
            var mockInjProtector = new Mock<IInjectionDetector>();
            var initialObject = new TestObject();

            // Act
            _ = GetSut(
                injectionDetector: mockInjProtector.Object,
                initialObject: initialObject);

            // Assert
            mockInjProtector.Verify(p => p.NotifyChanges(initialObject), Times.Once);
        }

        [Test]
        public void Constructor_InitialValueIsNull_NotifiesInnerInjectionDetector()
        {
            // Arrange
            var mockInjProtector = new Mock<IInjectionDetector>();

            // Act
            _ = GetSut(injectionDetector: mockInjProtector.Object);

            // Assert
            mockInjProtector.Verify(p => p.NotifyChanges(It.IsAny<TestObject>()), Times.Once);
        }

        [Test]
        [TestCaseSource(typeof(SafeObjectCases), nameof(SafeObjectCases.ProtectionModeAndProtectVariables))]
        public void Constructor_Sets_Inner_InjectionDetectorSettings_From_ProtectionMode(
            SafeObjectProtectionMode protectionMode, bool scanState, bool scanCode)
        {
            // Arrange
            var injectionDetector = Mock.Of<IInjectionDetector>();
            injectionDetector.ScanCode = !scanCode;
            injectionDetector.ScanState = !scanState;

            // Act
            _ = GetSut(protectionMode: protectionMode, injectionDetector: injectionDetector);

            // Assert
            Assert.That(injectionDetector.ScanState, Is.EqualTo(scanState));
            Assert.That(injectionDetector.ScanCode, Is.EqualTo(scanCode));
        }

        [Test]
        [TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
        public void Constructor_Sets_InnerInjectionDetectorsAlertChannel(InjectionAlertChannel alertChannel)
        {
            // Arrange
            var expected = alertChannel;
            var sut = GetSut(channel: expected);

            // Act
            var actual = sut.AlertChannel;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void MakeReadOnly_Sets_IsReadOnly_Property_To_True()
        {
            // Arrange
            const bool expected = true;
            var initialObject = new TestObject();
            var sut = GetSut(isReadOnly: false, initialObject: initialObject);

            // Act
            sut.MakeReadOnly();

            // Assert
            Assert.That(sut.IsReadOnly, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(SafeObjectCases), nameof(SafeObjectCases.ProtectionModeAndProtectVariables))]
        public void SetProtectionMode_Sets_Inner_InjectionDetectorSettings(SafeObjectProtectionMode protectionMode,
            bool scanState, bool scanCode)
        {
            // Arrange
            var injectionDetector = Mock.Of<IInjectionDetector>();
            injectionDetector.ScanCode = !scanCode;
            injectionDetector.ScanState = !scanState;

            // Act
            var sut = GetSut(protectionMode: protectionMode, injectionDetector: injectionDetector);
            sut.SetProtectionMode(protectionMode);

            // Assert
            Assert.That(injectionDetector.ScanState, Is.EqualTo(scanState));
            Assert.That(injectionDetector.ScanCode, Is.EqualTo(scanCode));
        }

        [Test]
        public void CanAlert_ForNonProtectedMode_returnsFalse()
        {
            // Arrange
            const bool expected = false;
            var injectionDetector = new Mock<IInjectionDetector>();
            injectionDetector.Setup(d => d.CanAlert).Returns(true);
            const SafeObjectProtectionMode nonProtectedMode = SafeObjectProtectionMode.NoProtection;
            var sut = GetSut(protectionMode: nonProtectedMode, injectionDetector: injectionDetector.Object);

            // Act
            var actual = sut.CanAlert;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(SafeObjectCases), nameof(SafeObjectCases.AlertingProtectionModes))]
        public void CanAlert_ForProtectModes_returnsTrue(SafeObjectProtectionMode protectionMode)
        {
            // Arrange
            const bool expected = true;
            var injectionDetector = new Mock<IInjectionDetector>();
            injectionDetector.Setup(d => d.CanAlert).Returns(true);

            // Act
            var sut = GetSut(protectionMode: protectionMode, injectionDetector: injectionDetector.Object);
            var actual = sut.CanAlert;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
        public void AlertChannel_Sets_InnerInjectionDetectorChannel(InjectionAlertChannel alertChannel)
        {
            // Arrange
            var expected = alertChannel;
            var injectionDetector = new Mock<IInjectionDetector>();
            var sut = GetSut(injectionDetector: injectionDetector.Object);

            // Act
            sut.AlertChannel = expected;

            // Assert
            injectionDetector.VerifySet(
                detector => detector.AlertChannel = expected
            );
        }

        [Test]
        [TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
        public void AlertChannel_Gets_InnerInjectionDetectorChannel(InjectionAlertChannel alertChannel)
        {
            //arrange
            var expected = alertChannel;
            var injectionDetector = new Mock<IInjectionDetector>();
            injectionDetector.SetupGet(detector => detector.AlertChannel)
                .Returns(expected);
            var sut = GetSut(injectionDetector: injectionDetector.Object);

            //act
            var actual = sut.AlertChannel;

            //assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Object_AfterCreatedWithInitialObject_returnsInitialObject()
        {
            // Arrange
            var expected = new TestObject();
            var sut = GetSut(expected);

            // Act
            var actual = sut.Object;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Object_ChangingItsValueAndVerifyingFromDifferentThreads_IsThreadSafe()
        {
            // Arrange
            var failed = false;
            var sut = GetSut();
            const int iterations = 100;

            // Act
            var thread1 = new Thread(() =>
            {
                for (var i = 0; i < iterations; i++)
                {
                    sut.ApplyChanges(obj => obj.TestProperty = i.ToString());
                    if (sut.Object.TestProperty != i.ToString())
                        failed = true;
                }
            });
            var thread2 = new Thread(() =>
            {
                for (var i = 0; i < iterations; i++)
                {
                    sut.ApplyChanges(obj => obj.TestInt = i);
                    if (sut.Object.TestInt != i)
                        failed = true;
                }
            });
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            // Assert
            Assert.IsFalse(failed, "code was thread safe");
        }

        [Test]
        public void Object_WhenRetrieving_IfChangesAreVerified_returnsChangedObject()
        {
            // Arrange
            var sut = GetSut();

            // Act
            sut.ApplyChanges(obj => obj.TestProperty = "change_without_verify");

            // Assert
            Assert.That(sut.Object.TestProperty, Is.EqualTo("change_without_verify"));
        }

        [Test]
        public void Object_WhenRetrieving_IfObjectChangedWithoutVerify_alertsInjection()
        {
            // Arrange
            var initialObject = new TestObject();
            var detectorMock = new Mock<IInjectionDetector>();
            detectorMock.Setup(c => c.CanAlert).Returns(true);
            var sut = GetSut(
                protectionMode: SafeObjectProtectionMode.StateAndCode,
                injectionDetector: detectorMock.Object,
                initialObject: initialObject);
            // Act
            initialObject.TestInt += 1;
            _ = sut.Object;

            // Assert
            detectorMock.Verify(detector => detector.AlertUnnotifiedChanges(initialObject));
        }

        [Test]
        public void Object_WhenRetrieving_IfObjectChangedWithoutVerify_InjectionDetectorCanNotAlert_doesNotAlert()
        {
            // Arrange
            var initialObject = new TestObject();
            var detectorMock = new Mock<IInjectionDetector>();
            detectorMock.Setup(c => c.CanAlert).Returns(false);
            var sut = GetSut(
                protectionMode: SafeObjectProtectionMode.StateAndCode,
                injectionDetector: detectorMock.Object,
                initialObject: initialObject);

            // Act
            initialObject.TestInt += 1;
            _ = sut.Object;

            // Assert
            detectorMock.Verify(detector => detector.AlertUnnotifiedChanges(initialObject), Times.Never);
        }


        /// <seealso cref="ReadOnlyAccessForbiddenException" />
        [Test]
        public void ApplyChanges_IfObjectIsNotModifiable_throwsReadOnlyAccessForbiddenException()
        {
            // Arrange
            var initialObject = new TestObject();
            var sut = GetSut(initialObject);
            sut.MakeReadOnly();

            // Act
            void CallVerifyChanges() => sut.ApplyChanges(a => { });

            // Assert
            Assert.That(CallVerifyChanges, Throws.TypeOf<ReadOnlyAccessForbiddenException>());
        }

        [Test]
        public void ApplyChanges_ForInnerInjectionDetector_InvokesNotifyChanges()
        {
            // Arrange
            var detectorMock = new Mock<IInjectionDetector>();
            var sut = GetSut(injectionDetector: detectorMock.Object);
            detectorMock.Reset();
            var testObj = sut.Object;

            // Act
            sut.ApplyChanges(obj => obj.TestInt = 5);

            // Assert
            detectorMock.Verify(detector => detector.NotifyChanges(It.Is<TestObject>(t => t == testObj)),
                Times.Once);
        }

        [Test]
        public void ApplyChanges_ChangesTheObject()
        {
            // Arrange
            var expectedInt = 10;
            var expectedString = "after";
            var testObj = new TestObject {TestInt = 5, TestProperty = "before"};
            var sut = GetSut(initialObject: testObj);

            // Act
            sut.ApplyChanges(obj =>
            {
                obj.TestInt = expectedInt;
                obj.TestProperty = expectedString;
            });
            var actual = sut.Object;

            // Assert
            Assert.That(actual.TestInt, Is.EqualTo(expectedInt));
            Assert.That(actual.TestProperty, Is.EqualTo(expectedString));
        }

        [Test]
        [TestCaseSource(typeof(SafeObjectCases), nameof(SafeObjectCases.NonStateProtectionToStateProtectionCases))]
        public void SetProtectionMode_From_NonStateProtection_To_StateProtection_NotifiesInnerInjectionDetector(
            SafeObjectProtectionMode from, SafeObjectProtectionMode to)
        {
            // Arrange
            var detectorMock = new Mock<IInjectionDetector>();
            detectorMock.SetupProperty(d => d.ScanState);
            var sut = GetSut(protectionMode: from,
                injectionDetector: detectorMock.Object);
            var obj = sut.Object;
            detectorMock.Invocations.Clear();

            //  Act
            sut.SetProtectionMode(to);

            // Assert
            detectorMock.Verify(d => d.NotifyChanges(obj), Times.Once);
        }

        [Test]
        [TestCaseSource(typeof(SafeObjectCases), nameof(SafeObjectCases.StateProtectionToNonStateProtectionCases))]
        public void SetProtectionMode_From_StateProtection_To_NonStateProtection_DoesNotNotifyInnerInjectionDetector(
            SafeObjectProtectionMode from, SafeObjectProtectionMode to)
        {
            // Arrange
            var detectorMock = new Mock<IInjectionDetector>();
            var sut = GetSut(protectionMode: from,
                injectionDetector: detectorMock.Object);
            var obj = sut.Object;
            detectorMock.Invocations.Clear();

            // Act
            sut.SetProtectionMode(to);

            // Assert
            detectorMock.Verify(d => d.NotifyChanges(obj), Times.Never);
        }

        public SafeObject<TestObject> GetSut(
            TestObject initialObject = null,
            bool isReadOnly = false,
            SafeObjectProtectionMode protectionMode = Defaults.ObjectProtectionMode,
            IInjectionDetector injectionDetector = null,
            InjectionAlertChannel channel = Defaults.AlertChannel)
        {
            return
                new SafeObject<TestObject>(
                    new InitialSafeObjectSettings(
                        isReadOnly: isReadOnly, initialValue: initialObject,
                        protectionMode: protectionMode, alertChannel: channel)
                    , injectionDetector ?? Stubs.Get<IInjectionDetector>());
        }

        public sealed class TestObject : IEquatable<TestObject>
        {
            public int TestInt { get; set; }
            public string TestProperty { get; set; } = "TestValue";
            public bool Equals(TestObject other) => TestProperty == other?.TestProperty;
        }

        /// <summary>
        ///     Gets the protection mode test cases where first argument is the sut and the second is
        ///     <see cref="SafeObjectProtectionMode" /> to be set and expected.
        /// </summary>
        /// <seealso cref="ProtectableBaseTests{TProtectionMode}" />
        protected override IEnumerable<TestCaseData> GetProtectionModeTestCases()
        {
            yield return
                new TestCaseData(GetSut(),
                    SafeObjectProtectionMode.JustState);
            yield return
                new TestCaseData(GetSut(),
                    SafeObjectProtectionMode.JustCode);
            yield return
                new TestCaseData(GetSut(),
                    SafeObjectProtectionMode.NoProtection);
            yield return
                new TestCaseData(GetSut(protectionMode: SafeObjectProtectionMode.JustState),
                    SafeObjectProtectionMode.StateAndCode);
            yield return
                new TestCaseData(GetSut(protectionMode: SafeObjectProtectionMode.JustState),
                    SafeObjectProtectionMode.JustCode);
            yield return
                new TestCaseData(GetSut(protectionMode: SafeObjectProtectionMode.JustState),
                    SafeObjectProtectionMode.NoProtection);
            yield return
                new TestCaseData(GetSut(protectionMode: SafeObjectProtectionMode.JustCode),
                    SafeObjectProtectionMode.StateAndCode);
            yield return
                new TestCaseData(GetSut(protectionMode: SafeObjectProtectionMode.JustCode),
                    SafeObjectProtectionMode.JustState);
            yield return
                new TestCaseData(GetSut(protectionMode: SafeObjectProtectionMode.JustCode),
                    SafeObjectProtectionMode.NoProtection);
            yield return
                new TestCaseData(GetSut(protectionMode: SafeObjectProtectionMode.NoProtection),
                    SafeObjectProtectionMode.JustCode);
            yield return
                new TestCaseData(GetSut(protectionMode: SafeObjectProtectionMode.NoProtection),
                    SafeObjectProtectionMode.JustState);
            yield return
                new TestCaseData(GetSut(protectionMode: SafeObjectProtectionMode.NoProtection),
                    SafeObjectProtectionMode.StateAndCode);
        }
    }
}