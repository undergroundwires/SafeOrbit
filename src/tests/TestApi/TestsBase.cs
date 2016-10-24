using System;
using System.Diagnostics;
using System.Linq;
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
        /// Measures the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>Total elapsed milliseconds</returns>
        protected long Measure(Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            long elapsedTime;
            try
            {
                action.Invoke();
            }
            finally
            {
                stopwatch.Stop();
                elapsedTime = stopwatch.ElapsedMilliseconds;
            }
            return elapsedTime;
        }
    }
}