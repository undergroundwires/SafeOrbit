using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SafeOrbit.Memory;
using SafeOrbit.Random;
using SafeOrbit.Tests;

namespace SafeOrbit.UnitTests.Random
{
    [TestFixture]
    public class FastRandomTests : TestsFor<IFastRandom>
    {
        [Test]
        public void StaticInstance_InMultipleThreads_CanBeAccessedAndConsumed()
        {
            var sut = FastRandom.StaticInstance;
            var iterations = 100;
            var bufferLength = 200; //high for collisions
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
                    tempBytes = sutInThread.GetBytes(100);
                    Assert.That(tempBytes, Is.Not.Null);
                    Assert.That(tempBytes, Is.Not.Empty);
                    Assert.That(tempBytes, Has.Length.EqualTo(bufferLength));
                });
            }
            for (int i = 0; i < iterations; i++)
            {
                threads[i].Start();
            }
            for (int i = 0; i < iterations; i++)
            {
                threads[i].Join();
            }
            Assert.That(byteList.Distinct().Count(), Is.EqualTo(iterations));
        }
        protected override IFastRandom GetSut()
        {
            return new FastRandom();
        }
    }
}