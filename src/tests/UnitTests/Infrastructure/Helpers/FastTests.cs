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
        public void FastFor_1000Factorials_FasterThanForLoop()
        {
            //arrange
            var iterations = 1000;
            Func<int, int> factorial = n => n == 0 ? 1 :
                Enumerable.Range(1, n).Aggregate((acc, x) => acc * x);
            var expectedMax = base.Measure(() =>
            {
                for (int i = 0; i < iterations; i++)
                {
                    factorial.Invoke(i);
                }
            });
            //act
            var actual = base.Measure(() =>
            {
                Fast.For(0, iterations, (i) => factorial.Invoke(i));
            });
            //assert
            Assert.That(actual, Is.LessThan(expectedMax));
        }
    }
}
