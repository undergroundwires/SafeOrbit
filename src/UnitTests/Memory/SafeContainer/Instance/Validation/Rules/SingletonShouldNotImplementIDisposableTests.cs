
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeOrbit.Memory;
using SafeOrbit.Memory.SafeContainerServices.Instance.Providers;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Validation
{
    /// <seealso cref="SingletonShouldNotImplementIDisposableTests"/>
    /// <seealso cref="IInstanceProviderRule"/>
    internal class SingletonShouldNotImplementIDisposableTests : TestsFor<SingletonShouldNotImplementIDisposable>
    {
        [Test]
        public void WhenSingletonImplementsIDisposable_ReturnsError()
        {
            //arrange
            var sut = GetSut();
            var instance = new SingletonInstanceProvider<DisposableClass>(InstanceProtectionMode.NoProtection);
            //act
            var isSatisfied = sut.IsSatisfiedBy(instance);
            var errors = sut.Errors;
            //assert
            Assert.That(isSatisfied, Is.False);
            Assert.That(errors, Is.Not.Null.Or.Empty);
            Assert.That(errors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void WhenSingletonDoesNotImplementIDisposable_DoesNotReturnError()
        {
            //arrange
            var sut = GetSut();
            var instance = new SingletonInstanceProvider<NotDisposableClass>(InstanceProtectionMode.NoProtection);
            //act
            var isSatisfied = sut.IsSatisfiedBy(instance);
            var errors = sut.Errors;
            //assert
            Assert.That(isSatisfied, Is.True);
            Assert.That(errors, Is.Null.Or.Empty);
        }


        protected override SingletonShouldNotImplementIDisposable GetSut()
        {
            return new SingletonShouldNotImplementIDisposable();
        }
        private class NotDisposableClass { }
        private class DisposableClass : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
