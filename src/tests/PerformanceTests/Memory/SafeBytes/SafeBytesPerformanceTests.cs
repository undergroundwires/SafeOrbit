﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Library;
using SafeOrbit.Memory;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory
{
    /// <seealso cref="ISafeBytes" />
    /// <seealso cref="SafeBytes" />
    [TestFixture]
    public class SafeBytesPerformanceTests : TestsFor<ISafeBytes>
    {
        [Test]
        public void ToByteArray_For_100_Bytes_Takes_Less_Than_3000ms()
        {
            //arrange
            SafeOrbitCore.Current.StartEarly();
            var sut = GetSut();
            var expectedHigherLimit = 3000;
            for (var i = 0; i < 100; i++)
            {
                sut.Append((byte)i);
            }
            //act
            var actualPerformance = base.Measure(
                () => sut.ToByteArray());
            //assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }
        [Test]
        public void Adding_100_Bytes_Takes_Less_Than_200ms()
        {
            //arrange
            SafeOrbitCore.Current.StartEarly();
            var sut = GetSut();
            var expectedHigherLimit = 200;
            //act
            long actualPerformance = base.Measure(() =>
            {
                for (var i = 0; i < 100; i++)
                {
                    sut.Append((byte)i);
                }
            });
            //assert
            Assert.That(actualPerformance, Is.LessThanOrEqualTo(expectedHigherLimit));
        }
        protected override ISafeBytes GetSut()
        {
            return new SafeBytes();
        }
    }
}
