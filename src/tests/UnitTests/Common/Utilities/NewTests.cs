
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
