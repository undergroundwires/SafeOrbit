using System;
using System.Collections.Generic;
using NUnit.Framework;
using SafeOrbit.Core.Protectable;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.Common.ProtectionLevelSwitch
{
    [TestFixture]
    public abstract class ProtectableBaseTests<TProtectionMode> : TestsBase
        where TProtectionMode : Enum
    {
        /// <summary>
        ///     Gets the protection mode test cases where first argument is the sut and the second is
        ///     <see cref="TProtectionMode" /> to be set and expected.
        /// </summary>
        protected abstract IEnumerable<TestCaseData> GetProtectionModeTestCases();

        [Test]
        public void SetProtectionMode_SetsTheCurrentProtectionMode()
        {
            var testCases = GetProtectionModeTestCases();
            foreach (var testCase in testCases)
            {
                //arrange
                var sut = (IProtectable<TProtectionMode>) testCase.Arguments[0];
                var expected = (TProtectionMode) testCase.Arguments[1];
                //act
                sut.SetProtectionMode(expected);
                var actual = sut.CurrentProtectionMode;
                //assert
                Assert.That(actual, Is.EqualTo(expected));
            }
        }
    }
}