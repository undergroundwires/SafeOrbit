
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SafeOrbit.Tests;

namespace SafeOrbit.Cryptography.Random
{
    /// <seealso cref="IFastRandom" />
    /// <seealso cref="FastRandom" />
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