
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Fakes;
using SafeOrbit.Memory;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Providers
{
    /// <seealso cref="CustomInstanceProvider{TImplementation}"/>
    /// <seealso cref="IInstanceProvider"/>
    [TestFixture]
    public class CustomInstanceProviderTests
    {
        [Test]
        public void Default_LifeTime_Is_Unknown()
        {
            //arrange
            var expected = LifeTime.Unknown;
            var dummyObject = new DateTime();
            var sut = GetSut(() => dummyObject);
            //act
            var actual = sut.LifeTime;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test, TestCaseSource(typeof(InstanceCases), nameof(InstanceCases.LifeTimes))]
        public void Constructor_Sets_LifeTime(LifeTime lifeTime)
        {
            //arrange
            var expected = lifeTime;
            var dummyObject = new DateTime();
            var sut = GetSut(() => dummyObject, lifeTime);
            //act
            var actual = sut.LifeTime;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test, TestCaseSource(typeof(CustomInstanceProviderTests), nameof(CustomInstanceProviderTests.LifeTime_Argument_Returns_CanProtectState))]
        public bool CanProtectState_DifferentLifeTimes_ReturnsVector(LifeTime lifeTime)
        {
            var dummyObject = new DateTime();
            var sut = GetSut(() => dummyObject, lifeTime);
            return sut.CanProtectState;
        }
        [Test]
        public void GetInstance_Returns_Instance_Of_Argument()
        {
            //arrange
            var expected = typeof(DateTime);
            var dummyObject = new DateTime();
            var sut = GetSut(() => dummyObject);
            //act
            var actual = sut.GetInstance();
            //assert
            Assert.That(actual, Is.InstanceOf(expected));
        }
        [Test]
        public void GetInstance_Returns_The_Result_Of_Func_Each_time()
        {
            //arrange
            var dummyObject = new InstanceTestClass();
            var func = new Func<InstanceTestClass>(() => dummyObject);
            var sut = GetSut(func);
            var expected = sut.GetInstance();
            //act
            var instance1 = sut.GetInstance();
            var instance2 = sut.GetInstance();
            var instance3 = sut.GetInstance();
            //assert
            Assert.That(expected, Is.EqualTo(instance1));
            Assert.That(expected, Is.EqualTo(instance2));
            Assert.That(expected, Is.EqualTo(instance3));
        }

        private static CustomInstanceProvider<T> GetSut<T>(Func<T> func, LifeTime? lifeTime = null) where T:new()
        {
            return
                lifeTime.HasValue
                    ? new CustomInstanceProvider<T>(
                        lifeTime: lifeTime.Value,
                        instanceGetter: func,
                        protectionMode: InstanceProtectionMode.NoProtection,
                        injectionDetector: Stubs.Get<IInjectionDetector>(),
                        alertChannel: InjectionAlertChannel.ThrowException)
                    : new CustomInstanceProvider<T>(
                        instanceGetter: func,
                        protectionMode: InstanceProtectionMode.NoProtection,
                        injectionDetector: Stubs.Get<IInjectionDetector>(),
                        alertChannel: InjectionAlertChannel.ThrowException);
        }

        private static IEnumerable<TestCaseData> LifeTime_Argument_Returns_CanProtectState
        {
            get
            {
                yield return new TestCaseData(LifeTime.Singleton).Returns(true);
                yield return new TestCaseData(LifeTime.Transient).Returns(false);
                yield return new TestCaseData(LifeTime.Unknown).Returns(false);
            }
        }
    }
}
