using System;
using System.Linq;
using NUnit.Framework;
using SafeOrbit.Tests;

namespace SafeOrbit.Helpers
{
    /// <seealso cref="Fast"/>
    [TestFixture]
    public class FastTests : TestsBase
    {
        [Test]
        public void FastFor_1000Factorials_FasterThanOrEqualToForLoop()
        {
            //arrange
            const int iterations = 1000;
            static int Factorial(int n) => n == 0 ? 1 :
                Enumerable.Range(1, n).Aggregate((acc, x) => acc * x);
            var expectedMax = base.Measure(() =>
            {
                for (var i = 0; i < iterations; i++)
                {
                    Factorial(i);
                }
            });
            //act
            var actual = base.Measure(() =>
            {
                Fast.For(0, iterations, (i) => Factorial(i));
            });
            //assert
            Assert.That(actual, Is.LessThanOrEqualTo(expectedMax));
        }
    }
}
