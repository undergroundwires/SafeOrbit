using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Utilities;

namespace SafeOrbit.UnitTests.Utilities
{
    [TestFixture]
    public class NewTests
    {
        [Test]
        public void Instance_creates_a_new_instance()
        {
            var instance = New<TestObject>.Instance.Invoke();
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.InstanceOf<TestObject>());
        }
        [Test]
        public void Instance_creates_new_instance_on_each_call()
        {
            var instance1 = New<TestObject>.Instance.Invoke();
            var instance2 = New<TestObject>.Instance.Invoke();
            var instance3 = New<TestObject>.Instance.Invoke();
            //assert
            Assert.That(instance1, Is.Not.EqualTo(instance2));
            Assert.That(instance1, Is.Not.EqualTo(instance3));
            Assert.That(instance2, Is.Not.EqualTo(instance3));
        }

        public class TestObject : IEquatable<TestObject>
        {
            private static int _idCounter;
            public TestObject()
            {
                Id = _idCounter;
                _idCounter++;
            }
            public int Id { get; }
            public bool Equals(TestObject other) => this.Id == other.Id;
        }
    }
}
