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