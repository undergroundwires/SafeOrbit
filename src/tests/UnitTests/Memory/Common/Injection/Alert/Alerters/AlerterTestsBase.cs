
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
using NUnit.Framework;
using SafeOrbit.Memory.Injection;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Memory.InjectionServices.Alerters;
using SafeOrbit.Tests;

namespace SafeOrbit.Memory.InjectionServices.Alerters
{
    [TestFixture]
    internal abstract class AlerterTestsBase<T> : TestsFor<T> where T: class, IAlerter
    {
        public abstract InjectionAlertChannel CoveredChannel { get; }

        [Test]
        public void Channel_Is_CoveredChannel()
        {
            var expected = CoveredChannel;
            var sut = GetSut();
            var actual = sut.Channel;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(typeof(Cases), nameof(Cases.MessageCases))]
        public void Asserts_The_Message(IInjectionMessage message)
        {
            var sut = GetSut();
            var action = new TestDelegate(() => sut.Alert(message));
            Alert_Sut_Alerts_Message(action, message);
        }

        public abstract void Alert_Sut_Alerts_Message(TestDelegate alertingMessage, IInjectionMessage message);
    }

    public class Cases
    {
        
        public static IEnumerable<IInjectionMessage> MessageCases
    {
        get
        {
            yield return new InjectionMessage(true, true, 5);
            yield return new InjectionMessage(true, false, "aq");
            yield return new InjectionMessage(false, true, new byte[] { 5, 10, 15 });
        }
    }
}
}