using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SafeOrbit.Cryptography.Random
{
    /// <summary>
    ///     Shared tests for <see cref="FastRandom" /> and <see cref="SafeRandom" />.
    /// </summary>
    /// <seealso cref="FastRandom" />
    /// <seealso cref="SafeRandom" />
    /// <seealso cref="FastRandomTests" />
    /// <seealso cref="SafeRandomTests" />
    [TestFixture]
    public abstract class CommonRandomTests<TRandom> where TRandom : ICryptoRandom
    {
        protected abstract TRandom GetStaticInstance();

        [Test]
        [Explicit]
        public void StaticInstance_InMultipleThreads_CanBeAccessedAndConsumed()
        {
            var sut = GetStaticInstance();
            const int totalThreads = 100;
            const int bufferLength = 200; //high for collisions
            var byteList = new ConcurrentBag<byte[]>();
            var threads = new Thread[totalThreads];
            for (var i = 0; i < totalThreads; i++)
                threads[i] = new Thread(() =>
                    RunAction(sut, bufferLength, byteList));
            foreach (var thread in threads)
                thread.Start();
            foreach (var thread in threads)
                thread.Join();
            Assert.That(byteList.Distinct().Count(), Is.EqualTo(totalThreads));
        }

        [Test]
        [Explicit]
        public void StaticInstance_InParallel_CanBeAccessedAndConsumed()
        {
            var sut = GetStaticInstance();
            const int iterations = 100;
            const int bufferLength = 200; //high for collisions
            var byteList = new ConcurrentBag<byte[]>();
            var actions = new Action[iterations];
            for (var i = 0; i < iterations; i++)
                actions[i] = () => RunAction(sut, bufferLength, byteList);
            System.Threading.Tasks.Parallel.Invoke(actions);
            Assert.That(byteList.Distinct().Count(), Is.EqualTo(iterations));
        }

        private void RunAction(TRandom existingInstance, int bufferLength, IProducerConsumerCollection<byte[]> byteList)
        {
            Assert.True(
                byteList.TryAdd(
                    existingInstance // We first try with already used getter
                        .GetBytes(bufferLength)
                )
            );
            Assert.True(
                byteList.TryAdd(
                    GetStaticInstance() // We then test with using the getter again
                        .GetBytes(bufferLength)));
        }
    }
}