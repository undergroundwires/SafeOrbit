
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

using System.Runtime.Serialization;
using NUnit.Framework;

namespace SafeOrbit.Memory.InjectionServices
{
    /// <seealso cref="InstanceIdGenerator"/>
    /// <seealso cref="IObjectIdGenerator"/>
    [TestFixture]
    public class InstanceIdGeneratorTests
    {
        [Test]
        public void GetId_SameInstances_ReturnsSameId()
        {
            //arrange
            var sut = GetSut();
            var instance = new IdGenerationTestClass();
            var instance2 = instance;
            //act
            var id1 = sut.GetStateId(instance);
            var id2 = sut.GetStateId(instance2);
            //assert
            Assert.That(id1, Is.EqualTo(id2));
        }

        [Test]
        public void GetId_SameTypesButDifferentInstances_ReturnsDifferentId()
        {
            //arrange
            var sut = GetSut();
            var instance = new IdGenerationTestClass();
            var instance2 = new IdGenerationTestClass();
            //act
            var id1 = sut.GetStateId(instance);
            var id2 = sut.GetStateId(instance2);
            //assert
            Assert.That(id1, Is.Not.EqualTo(id2));
        }

        private static IObjectIdGenerator GetSut() => new InstanceIdGenerator();

        private class IdGenerationTestClass
        {
            
        }
    }
}