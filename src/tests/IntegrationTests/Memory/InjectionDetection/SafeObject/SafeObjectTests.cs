using System.Collections.ObjectModel;
using NUnit.Framework;
using SafeOrbit.Exceptions;
using SafeOrbit.Library;
using SafeOrbit.Memory.Injection;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory
{
    /// <seealso cref="SafeObject{TObject}" />
    /// <seealso cref="ISafeObject{TObject}" />
    /// <seealso cref="SafeObjectProtectionMode" />
    [TestFixture]
    public class SafeObjectTests
    {
        [Test]
        public void SafeObject_PropertyChangedAfterVerifiedChanges_ThrowsMemoryInjectionException()
        {
            // Arrange
            SafeOrbitCore.Current.AlertChannel = InjectionAlertChannel.ThrowException;
            const string expected = "PropertyData";
            var sut = new SafeObject<TestClass>();
            sut.ApplyChanges(
                obj => obj.Class = new TestClass { Property = expected }
            );
            sut.Object.Class.Property = $"{expected}_changed";

            // Act
            void CallGetter() => _ = sut.Object;

            // Assert
            Assert.Throws<MemoryInjectionException>(CallGetter);
        }

        [Test]
        public void SafeObject_PropertyChangedAfterVerifiedChanges_RaisesLibraryInjectedEvent()
        {
            // Arrange
            var events = new Collection<IInjectionMessage>();
            SafeOrbitCore.Current.AlertChannel = InjectionAlertChannel.RaiseEvent;
            SafeOrbitCore.Current.LibraryInjected += (sender, args) => events.Add(args);
            const string expected = "PropertyData";
            var sut = new SafeObject<TestClass>();
            sut.ApplyChanges(
                obj => obj.Class = new TestClass { Property = expected }
            );
            sut.Object.Class.Property = $"{expected}_changed";

            // Act
            _ = sut.Object;

            // Assert
            Assert.AreEqual(1, events.Count);
        }


        private class TestClass
        {
            public TestClass Class { get; set; }
            public string Property { get; set; }
        }
    }
}