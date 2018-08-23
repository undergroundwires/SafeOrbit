
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

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
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace SafeOrbit.Infrastructure.Reflection
{
    /// <seealso cref="MethodInfoHelper"/>
    [TestFixture]
    public class MethodInfoHelperTests
    {
        [Test]
        public void GetIlBytes_ArgumentIsNull_throwsArgumentNullException()
        {
            //arrange
            var nullArgument = (MethodInfo)null;
            //act
            TestDelegate callingWithNull = () => nullArgument.GetIlBytes();
            //assert
            Assert.That(callingWithNull, Throws.ArgumentNullException);
        }
        [Test]
        public void GetIlBytes_TwiceForSameType_ReturnsSame()
        {
            //arrange
            var type = typeof(string);
            //act
            var expected = GetIlBytes(type);
            var actual = GetIlBytes(type);
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test]
        public void GetIlBytes_ForDifferentTypes_ReturnsDifferent()
        {
            //arrange
            var type = typeof(string);
            var differentType = typeof(int);
            //act
            var expected = GetIlBytes(type);
            var actual = GetIlBytes(differentType);
            //assert
            Assert.That(actual, Is.Not.EqualTo(expected));
        }

        private static byte[] GetIlBytes(Type type)
        {
            return type.GetMethods()
                .Where(m => m != null)
                .Select(m => m.GetIlBytes())
                .Where(arrays => arrays != null)
                .SelectMany(arrays => arrays) //merge arrays
                .ToArray();
        }
    }
}