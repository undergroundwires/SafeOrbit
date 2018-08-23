
/*
MIT License

Copyright (c) 2018 Erkin Ekici - undergroundwires@safeorb.it

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
using SafeOrbit.Fakes;
using SafeOrbit.Memory;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Providers
{
    /// <seealso cref="SingletonInstanceProvider{TImplementation}"/>
    /// <seealso cref="IInstanceProvider"/>
    [TestFixture]
    public class SingletonInstanceProviderTests
    {

        [Test]
        public void GetInstance_Returns_Instance_Of_Argument()
        {
            //arrange
            var expected = typeof(DateTime);
            var sut = GetSut<DateTime>();
            //act
            var actual = sut.GetInstance();
            //assert
            Assert.That(actual, Is.InstanceOf(expected));
        }

        [Test]
        public void CanProtectState_Is_True()
        {
            var sut = GetSut<DateTime>();
            Assert.That(sut.CanProtectState, Is.True);
        }

        [Test]
        public void GetInstance_Returns_Same_Instance_Each_Time()
        {
            //arrange
            var sut = GetSut<InstanceTestClass>();
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

        [Test]
        public void LifeTime_Is_Singleton()
        {
            //arrange
            var expected = LifeTime.Singleton;
            var sut = GetSut<object>();
            //act
            var actual = sut.LifeTime;
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        private static SingletonInstanceProvider<T> GetSut<T>() where T : new()
        {
            return new SingletonInstanceProvider<T>(
                protectionMode: InstanceProtectionMode.NoProtection,
                injectionDetector: Stubs.Get<IInjectionDetector>(),
                alertChannel: InjectionAlertChannel.ThrowException
                );
        }
    }
}