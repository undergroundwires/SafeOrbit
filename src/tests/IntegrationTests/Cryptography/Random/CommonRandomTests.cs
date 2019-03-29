using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Linq;

namespace SafeOrbit.Cryptography.Random
{
    /// <summary>
    /// Shared tests for <see cref="FastRandom"/> and <see cref="SafeRandom"/>.
    /// </summary>
    /// <seealso cref="FastRandom" />
    /// <seealso cref="SafeRandom" />
    /// <seealso cref="FastRandomTests" />
    /// <seealso cref="SafeRandomTests" />
    [TestFixture]
    public abstract class CommonRandomTests<TRandom> where TRandom: ICryptoRandom
    {
        protected abstract TRandom GetStaticInstance();
        [Test]
        public void StaticInstance_InMultipleThreads_CanBeAccessedAndConsumed()
        {
            var sut = GetStaticInstance();
            const int iterations = 100;
            const int bufferLength = 200; //high for collisions
            var byteList = new ConcurrentBag<byte[]>();
            var threads = new Thread[iterations];
            for (var i = 0; i < iterations; i++)
            {
                threads[i] = new Thread(() =>
                {
                    var tempBytes = sut.GetBytes(bufferLength);
                    Assert.That(tempBytes, Is.Not.Null);
                    Assert.That(tempBytes, Is.Not.Empty);
                    Assert.That(tempBytes, Has.Length.EqualTo(bufferLength));
                    byteList.Add(tempBytes);
                    var sutInThread = FastRandom.StaticInstance;
                    tempBytes = sutInThread.GetBytes(bufferLength);
                    Assert.That(tempBytes, Is.Not.Null);
                    Assert.That(tempBytes, Is.Not.Empty);
                    Assert.That(tempBytes, Has.Length.EqualTo(bufferLength));
                });
            }
            for (var i = 0; i < iterations; i++)
            {
                threads[i].Start();
            }
            for (var i = 0; i < iterations; i++)
            {
                threads[i].Join();
            }
            Assert.That(byteList.Distinct().Count(), Is.EqualTo(iterations));
        }

        [Test]
        public void StaticInstance_InParallel_CanBeAccessedAndConsumed()
        {
            var sut = GetStaticInstance();
            const int iterations = 100;
            const int bufferLength = 200; //high for collisions
            var byteList = new ConcurrentBag<byte[]>();
            var actions = new Action[iterations];
            for (var i = 0; i < iterations; i++)
            {
                actions[i] = () =>
                {
                    var tempBytes = sut.GetBytes(bufferLength);
                    Assert.That(tempBytes, Is.Not.Null);
                    Assert.That(tempBytes, Is.Not.Empty);
                    Assert.That(tempBytes, Has.Length.EqualTo(bufferLength));
                    byteList.Add(tempBytes);
                    var sutInThread = FastRandom.StaticInstance;
                    tempBytes = sutInThread.GetBytes(bufferLength);
                    Assert.That(tempBytes, Is.Not.Null);
                    Assert.That(tempBytes, Is.Not.Empty);
                    Assert.That(tempBytes, Has.Length.EqualTo(bufferLength));
                };
            }
            Parallel.Invoke(actions);
            Assert.That(byteList.Distinct().Count(), Is.EqualTo(iterations));
        }
    }
}