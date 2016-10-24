
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

using System.Collections.Generic;
using NUnit.Framework;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.Common.ProtectionLevelSwitch
{
    [TestFixture]
    public abstract class ProtectionLevelSwitchProviderBaseTests<TProtectionMode> : TestsBase
        where TProtectionMode : struct
    {
        /// <summary>
        /// Gets the protection mode test cases where first argument is the sut and the second is <see cref="TProtectionMode"/> to be set and expected.
        /// </summary>
        protected abstract IEnumerable<TestCaseData> GetProtectionModeTestCases();

        [Test]
        public void SetProtectionMode_SetsTheCurrentProtectionMode()
        {
            var testCases = GetProtectionModeTestCases();
            foreach (var testCase in testCases)
            {
                //arrange
                var sut = (IProtectionLevelSwitchProvider < TProtectionMode > )testCase.Arguments[0];
                var expected = (TProtectionMode)testCase.Arguments[1];
                //act
                sut.SetProtectionMode(expected);
                var actual = sut.CurrentProtectionMode;
                //assert
                Assert.That(actual, Is.EqualTo(expected));
            }
        }

    }
}