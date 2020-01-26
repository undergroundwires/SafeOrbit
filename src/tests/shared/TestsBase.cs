using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SafeOrbit.Tests
{
    public abstract class TestsBase
    {
        protected void IgnoreTest()
        {
            Assert.Inconclusive();
        }

        /// <summary>
        ///     Measures the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>Total elapsed milliseconds</returns>
        protected async Task<double> MeasureAsync(Func<Task> task, int repeat = 1)
        {
            if (repeat <= 0) throw new ArgumentOutOfRangeException(nameof(repeat));
            var results = new Collection<long>();
            for (var i = 0; i < repeat; i++)
            {
                var stopwatch = Stopwatch.StartNew();
                long elapsedTime;
                try
                {
                    await task.Invoke().ConfigureAwait(false);
                }
                finally
                {
                    stopwatch.Stop();
                    elapsedTime = stopwatch.ElapsedMilliseconds;
                }
                results.Add(elapsedTime);
            }
            var average = results.Average();
            Console.WriteLine($"Each run {string.Join(" , ", results)}. Average: {average} ms");
            return average;
        }

        protected double Measure(Action action, int repeat = 1)
        {
            return MeasureAsync(() =>
            {
                action();
                return Task.FromResult(true);
            }, repeat)
                .GetAwaiter()
                .GetResult();
        }
    }
}