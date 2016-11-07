using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace SafeOrbit.Common.Reflection
{
    /// <seealso cref="MethodInfoHelper"/>
    [TestFixture]
    public class MethodInfoHelperTests
    {
        [Test]
        public void GetIlBytes_ArgumentIsNull_throwsArgumentNullException()
        {
            //arrange
            var nullArgument = (MethodInfo) null;
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
            var expected = type.GetMethods().Select(m => m.GetIlBytes());
            var actual = type.GetMethods().Select(m => m.GetIlBytes());
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
            var expected = type.GetMethods().Select(m => m.GetIlBytes());
            var actual = differentType.GetMethods().Select(m => m.GetIlBytes());
            //assert
            Assert.That(actual, Is.Not.EqualTo(expected));
        }
    }
}