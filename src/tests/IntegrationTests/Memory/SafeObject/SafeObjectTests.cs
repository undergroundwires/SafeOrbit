
/*
MIT License

Copyright (c) 2018 Erkin Ekici - undergroundwires@safeorb.it

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