using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SafeOrbit.Parallel
{
    [TestFixture]
    public class AsyncLazyTests
    {
        [Test]
        public async Task AsyncLazy_returns_the_specified_value()
        {
            // Arrange
            const string expected = "hello!";
            var lazy = new AsyncLazy<string>(async () =>
            {
                await Task.Yield();
                return expected;
            });

            // Act
            var actual = await lazy.ValueAsync();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task AsyncLazy_can_be_awaited_concurrently_without_triggering_initialization_twice()
        {
            // Arrange
            var callCount = 0;
            var barrier = new Barrier(5);
            var lazy = new AsyncLazy<string>(async () =>
            {
                await Task.Yield();
                barrier.SignalAndWait(200);
                Interlocked.Increment(ref callCount);
                barrier.SignalAndWait(200);
                return "hello!";
            });

            // Act
            var calls = Enumerable.Range(1, 5)
                .Select(_ => lazy.ValueAsync());
            await Task.WhenAll(calls);

            // Assert
            Assert.AreEqual(1, callCount);
        }
    }
}
