using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace SafeOrbit.Core.Reflection
{
    /// <seealso cref="MethodInfoHelper" />
    [TestFixture]
    public class MethodInfoHelperTests
    {
        [Test]
        public void GetIlBytes_ArgumentIsNull_throwsArgumentNullException()
        {
            //arrange
            var nullArgument = (MethodInfo) null;

            //act
            void CallWithNull() => nullArgument.GetIlBytes();
            //assert
            Assert.That(CallWithNull, Throws.ArgumentNullException);
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