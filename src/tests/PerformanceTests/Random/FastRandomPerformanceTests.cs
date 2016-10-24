
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

using System;
using NUnit.Framework;
using SafeOrbit.Random;
using SafeOrbit.Tests;

namespace Core.PerformanceTests
{
    [TestFixture]
    public class FastRandomPerformanceTests : TestsFor<IFastRandom>
    {
        protected override IFastRandom GetSut()
        {
            return new FastRandom();
        }

        [Test]
        public void GetFastBytes_ForMultipleLittleBuffers_IsFasterThanSystemRandom()
        {
            //arrange
            var bufferLength = 100;
            var totalIterations = 10000;
            var sut = GetSut();
            var systemRandom = new Random();
            var resultFromSystemRandom = Measure(() =>
            {
                for (var i = 0; i < totalIterations; i++)
                {
                    var buffer = new byte[bufferLength];
                    systemRandom.NextBytes(buffer);
                }
            });
            //act
            var result = Measure(() =>
            {
                for (var i = 0; i < totalIterations; i++)
                    sut.GetBytes(bufferLength);
            });
            //assert
            Assert.That(result, Is.LessThanOrEqualTo(resultFromSystemRandom));
        }

        [Test]
        public void GetFastBytes_ForSingleBigBuffer_IsFasterThanSystemRandom()
        {
            //arrange
            var bufferLength = 1000000;
            var sut = GetSut();
            var systemRandom = new Random();
            var resultFromSystemRandom = Measure(() =>
            {
                var buffer = new byte[bufferLength];
                systemRandom.NextBytes(buffer);
            });
            ;
            //act
            var result = Measure(() => { sut.GetBytes(bufferLength); });
            //assert
            Assert.That(result, Is.LessThanOrEqualTo(resultFromSystemRandom));
        }

        [Test]
        public void GetInt_ForMultipleIterations_IsFasterThanSystemRandom()
        {
            //arrange
            var totalIterations = 1000000;
            var sut = GetSut();
            var systemRandom = new Random();
            var resultFromSystemRandom = Measure(() =>
            {
                for (var i = 0; i < totalIterations; i++)
                    systemRandom.Next();
            });
            //act
            var result = Measure(() =>
            {
                for (var i = 0; i < totalIterations; i++)
                    sut.GetInt();
            });
            //assert
            Assert.That(result, Is.LessThanOrEqualTo(resultFromSystemRandom));
        }

        [Test]
        public void GetInt_InAnInterval_ForMultipleIterations_IsFasterThanSystemRandom()
        {
            //arrange
            var totalIterations = 1000000;
            var lowerBoundary = 0;
            var higherBoundary = int.MaxValue;
            var sut = GetSut();
            var systemRandom = new Random();
            var resultFromSystemRandom = Measure(() =>
            {
                for (var i = 0; i < totalIterations; i++)
                    systemRandom.Next(lowerBoundary, higherBoundary);
            });
            //act
            var result = Measure(() =>
            {
                for (var i = 0; i < totalIterations; i++)
                    sut.GetInt(lowerBoundary, higherBoundary);
            });
            //assert
            Assert.That(result, Is.LessThanOrEqualTo(resultFromSystemRandom));
        }
    }
}