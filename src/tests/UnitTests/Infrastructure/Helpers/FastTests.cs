using System.Collections.Concurrent;
using System.Linq;
using NUnit.Framework;
using SafeOrbit.Helpers;

namespace UnitTests.Infrastructure.Helpers
{
    /// <seealso cref="Fast"/>
    [TestFixture]
    public class FastTests
    {
        [Test]
        public void For_ZeroToTen_EachIterationIsCalled()
        {
            // Arrange
            var iterations = new ConcurrentBag<int>();
            var expected = new [] { 0,1,2,3,4,5,6,7,8,9};
            // Act
            Fast.For(0, 10, 
                i => iterations.Add(i)
                );
            // Assert
            var actual = iterations.OrderBy(p => p).ToArray();
            Assert.True(expected.SequenceEqual(actual));
        }
    }
}
