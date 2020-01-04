using NUnit.Framework;
using SafeOrbit.Exceptions;

namespace SafeOrbit.Memory
{
    /// <seealso cref="SafeObject{TObject}" />
    /// <seealso cref="ISafeObject{TObject}" />
    /// <seealso cref="SafeObjectProtectionMode" />
    [TestFixture]
    public class SafeObjectTests
    {
        [Test]
        public void SafeObject_Object_WhenPropertyChangedAfterVerifiedChanges_throwsMemoryInjectionException()
        {
            //arrange
            var expected = "PropertyData";
            var sut = new SafeObject<TestClass>();
            sut.ApplyChanges(
                obj => obj.Class = new TestClass {Property = expected}
            );
            sut.Object.Class.Property = $"{expected}_changed";
            //act
            TestDelegate callingGetter = () =>
            {
                var temp = sut.Object;
            };
            //assert
            Assert.That(callingGetter, Throws.TypeOf<MemoryInjectionException>());
        }

        private class TestClass
        {
            public TestClass Class { get; set; }
            public string Property { get; set; }
        }
    }
}