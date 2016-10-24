using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using NUnit.Framework;
using SafeOrbit.Exceptions;
using SafeOrbit.Memory;
using SafeOrbit.Memory.Common;
using SafeOrbit.Memory.Common.ProtectionLevelSwitch;
using SafeOrbit.Memory.Injection;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.UnitTests;

namespace SafeOrbit.Memory
{
    /// <seealso cref="SafeObject{TObject}" />
    /// <seealso cref="ISafeObject{TObject}" />
    /// <seealso cref="SafeObjectProtectionMode" />
    /// <seealso cref="IProtectionLevelSwitchProvider{SafeObjectProtectionMode}"/>
    [TestFixture]
    public class SafeObjectTests : ProtectionLevelSwitchProviderBaseTests<SafeObjectProtectionMode>
    {
        [Test, TestCaseSource(typeof(SafeObjectCases), nameof(SafeObjectCases.SafeObjectProtectionModeCases))]
        public void Constructor_Sets_ProtectionMode(SafeObjectProtectionMode mode)
        {
            //arrange
            var expected = mode;
            //act
            var sut = GetSut(protectionMode: expected);
            var actual = sut.CurrentProtectionMode;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_Sets_InitialObject()
        {
            var initialObject = new TestObject {TestProperty = "changed"};
            var sut = GetSut(initialObject);
            Assert.That(initialObject, Is.EqualTo(sut.Object));
        }

        [Test, TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
        public void Constructor_Sets_AlertChannel(InjectionAlertChannel alertChannel)
        {
            //arrange
            var expected = alertChannel;
            //act
            var sut = GetSut(channel: expected);
            var actual = sut.AlertChannel;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_Sets_IsReadOnly()
        {
            var modifiableSut = GetSut(isReadOnly: false);
            var nonModifiableSut = GetSut(isReadOnly: true);
            Assert.That(modifiableSut.IsReadOnly, Is.False);
            Assert.That(nonModifiableSut.IsReadOnly, Is.True);
        }

        [Test]
        public void Constructor_WithoutInitialObject_CreatesANewInstance()
        {
            var sut = GetSut();
            var newInstance = sut.Object;
            Assert.That(newInstance, Is.Not.Null);
            Assert.That(newInstance, Is.InstanceOf<TestObject>());
        }

        [Test]
        public void Constructor_InitialValueIsNotNull_NotifiesInnerInjectionDetector()
        {
            var mockInjProtector = new Mock<IInjectionDetector>();
            var initialObject = new TestObject();
            var sut = GetSut(
                injectionDetector: mockInjProtector.Object,
                initialObject: initialObject);
            mockInjProtector.Verify(p => p.NotifyChanges(initialObject), Times.Once);
        }

        [Test]
        public void Constructor_InitialValueIsNull_NotifiesInnerInjectionDetector()
        {
            var mockInjProtector = new Mock<IInjectionDetector>();
            var sut = GetSut(
                injectionDetector: mockInjProtector.Object);
            mockInjProtector.Verify(p => p.NotifyChanges(It.IsAny<TestObject>()), Times.Once);
        }

        [Test, TestCaseSource(typeof(SafeObjectCases), nameof(SafeObjectCases.ProtectionModeAndProtectVariables))]
        public void Constructor_Sets_Inner_InjectionDetectorSettings_From_ProtectionMode(
            SafeObjectProtectionMode protectionMode, bool scanState, bool scanCode)
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
        public void Constructor_Sets_InnerInjectionDetectorsAlertChannel(InjectionAlertChannel alertChannel)
        {
            //arrange
            var expected = alertChannel;
            var sut = GetSut(channel: expected);
            //arrange
            var actual = sut.AlertChannel;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void MakeReadOnly_Sets_IsReadOnly_Property_To_True()
        {
            //arrange
            const bool expected = true;
            var initialObject = new TestObject();
            var sut = GetSut(isReadOnly: false, initialObject: initialObject);
            //act
            sut.MakeReadOnly();
            //assert
            Assert.That(sut.IsReadOnly, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(SafeObjectCases), nameof(SafeObjectCases.ProtectionModeAndProtectVariables))]
        public void SetProtectionMode_Sets_Inner_InjectionDetectorSettings(SafeObjectProtectionMode protectionMode,
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

        [Test]
        public void CanAlert_ForNonProtectedMode_returnsFalse()
        {
            var expected = false;
            var injectionDetector = new Mock<IInjectionDetector>();
            injectionDetector.Setup(d => d.CanAlert).Returns(true);
            var nonProtectedMode = SafeObjectProtectionMode.NoProtection;
            var sut = GetSut(protectionMode: nonProtectedMode, injectionDetector: injectionDetector.Object);
            var actual = sut.CanAlert;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(SafeObjectCases), nameof(SafeObjectCases.AlertingProtectionModes))]
        public void CanAlert_ForProtectModes_returnsTrue(SafeObjectProtectionMode protectionMode)
        {
            //arrange
            var expected = true;
            var injectionDetector = new Mock<IInjectionDetector>();
            injectionDetector.Setup(d => d.CanAlert).Returns(true);
            //act
            var sut = GetSut(protectionMode: protectionMode, injectionDetector: injectionDetector.Object);
            var actual = sut.CanAlert;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
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

        [Test, TestCaseSource(typeof(InjectionCases), nameof(InjectionCases.InjectionAlertChannelCases))]
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
        public void Object_AfterCreatedWithInitialObject_returnsInitialObject()
        {
            //arrange
            var expected = new TestObject();
            var sut = GetSut(expected);
            //act
            var actual = sut.Object;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Object_ChangingItsValueAndVerifyingFromDifferentThreads_IsThreadSafe()
        {
            var failed = false;
            var sut = GetSut();
            var iterations = 100;
            // threads interact with some object - either 
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
            Assert.IsFalse(failed, "code was thread safe");
        }

        [Test]
        public void Object_WhenRetrieving_IfChangesAreVerified_returnsChangedObject()
        {
            var sut = GetSut(isReadOnly: false);
            sut.ApplyChanges(obj => obj.TestProperty = "change_without_verify");
            Assert.That(sut.Object.TestProperty, Is.EqualTo("change_without_verify"));
        }

        [Test]
        public void Object_WhenRetrieving_IfObjectChangedWithoutVerify_alertsInjection()
        {
            //arrange
            var initialObject = new TestObject();
            var detectorMock = new Mock<IInjectionDetector>();
            detectorMock.Setup(c => c.CanAlert).Returns(true);
            var sut = GetSut(
                protectionMode: SafeObjectProtectionMode.StateAndCode,
                injectionDetector: detectorMock.Object,
                initialObject: initialObject);
            //act
            initialObject.TestInt += 1;
            var dummy = sut.Object;
            //assert
            detectorMock.Verify(detector => detector.AlertUnnotifiedChanges(initialObject));
        }

        [Test]
        public void Object_WhenRetrieving_IfObjectChangedWithoutVerify_InjectionDetectorCanNotAlert_doesNotAlert()
        {
            //arrange
            var initialObject = new TestObject();
            var detectorMock = new Mock<IInjectionDetector>();
            detectorMock.Setup(c => c.CanAlert).Returns(false);
            var sut = GetSut(
                protectionMode: SafeObjectProtectionMode.StateAndCode,
                injectionDetector: detectorMock.Object,
                initialObject: initialObject);
            //act
            initialObject.TestInt += 1;
            var dummy = sut.Object;
            //assert
            detectorMock.Verify(detector => detector.AlertUnnotifiedChanges(initialObject), Times.Never);
        }


        /// <seealso cref="ReadOnlyAccessForbiddenException"/>
        [Test]
        public void ApplyChanges_IfObjectIsNotModifiable_throwsReadOnlyAccessForbiddenException()
        {
            //arrange
            var initialObject = new TestObject();
            var sut = GetSut(initialObject);
            sut.MakeReadOnly();
            //act
            TestDelegate callVerifyChanges = () => sut.ApplyChanges(a => { });
            //assert
            Assert.That(callVerifyChanges, Throws.TypeOf<ReadOnlyAccessForbiddenException>());
        }

        [Test]
        public void ApplyChanges_ForInnerInjectionDetector_InvokesNotifyChanges()
        {
            //arrange
            var detectorMock = new Mock<IInjectionDetector>();
            var sut = GetSut(injectionDetector: detectorMock.Object);
            detectorMock.Reset();
            var testObj = sut.Object;
            //act
            sut.ApplyChanges(obj => obj.TestInt = 5);
            //assert
            detectorMock.Verify(detector => detector.NotifyChanges(It.Is<TestObject>(t => t == testObj)),
                Times.Once);
        }

        [Test]
        public void ApplyChanges_ChangesTheObject()
        {
            //arrange
            var expectedInt = 10;
            var expectedString = "after";
            var testObj = new TestObject {TestInt = 5, TestProperty = "before"};
            var sut = GetSut(initialObject: testObj);
            //act
            sut.ApplyChanges(obj =>
            {
                obj.TestInt = expectedInt;
                obj.TestProperty = expectedString;
            });
            var actual = sut.Object;
            //assert
            Assert.That(actual.TestInt, Is.EqualTo(expectedInt));
            Assert.That(actual.TestProperty, Is.EqualTo(expectedString));
        }

        [Test, TestCaseSource(typeof(SafeObjectCases), nameof(SafeObjectCases.NonStateProtectionToStateProtectionCases))]
        public void SetProtectionMode_From_NonStateProtection_To_StateProtection_NotifiesInnerInjectionDetector(SafeObjectProtectionMode from, SafeObjectProtectionMode to)
        {
            //arrange
            var detectorMock = new Mock<IInjectionDetector>();
            detectorMock.SetupProperty(d => d.ScanState);
            var sut = GetSut(protectionMode: from,
                injectionDetector: detectorMock.Object);
            var obj = sut.Object;
            detectorMock.ResetCalls();
            //act
            sut.SetProtectionMode(to);
            //assert
            detectorMock.Verify(d => d.NotifyChanges(obj), Times.Once);
        }

        [Test, TestCaseSource(typeof(SafeObjectCases), nameof(SafeObjectCases.StateProtectionToNonStateProtectionCases))]
        public void SetProtectionMode_From_StateProtection_To_NonStateProtection_DoesNotNotifyInnerInjectionDetector(SafeObjectProtectionMode from, SafeObjectProtectionMode to)
        {
            //arrange
            var detectorMock = new Mock<IInjectionDetector>();
            var sut = GetSut(protectionMode: from,
                injectionDetector: detectorMock.Object);
            var obj = sut.Object;
            detectorMock.ResetCalls();
            //act
            sut.SetProtectionMode(to);
            //assert
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

        public class TestObject : IEquatable<TestObject>
        {
            public int TestInt { get; set; }
            public string TestProperty { get; set; } = "TestValue";
            public bool Equals(TestObject other) => TestProperty == other?.TestProperty;
        }

        /// <summary>
        /// Gets the protection mode test cases where first argument is the sut and the second is <see cref="SafeObjectProtectionMode"/> to be set and expected.
        /// </summary>
        /// <seealso cref="ProtectionLevelSwitchProviderBaseTests{ProtectionMode}"/>
        protected override IEnumerable<TestCaseData> GetProtectionModeTestCases()
        {
            yield return
                new TestCaseData(GetSut(protectionMode: SafeObjectProtectionMode.StateAndCode),
                    SafeObjectProtectionMode.JustState);
            yield return
                new TestCaseData(GetSut(protectionMode: SafeObjectProtectionMode.StateAndCode),
                    SafeObjectProtectionMode.JustCode);
            yield return
                new TestCaseData(GetSut(protectionMode: SafeObjectProtectionMode.StateAndCode),
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