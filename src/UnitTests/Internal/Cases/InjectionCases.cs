
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
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory
{
    public class InjectionCases
    {
        public static InjectionAlertChannel[] InjectionAlertChannelCases => new[]
        {
            InjectionAlertChannel.DebugFail, InjectionAlertChannel.DebugWrite,
            InjectionAlertChannel.RaiseEvent,
            InjectionAlertChannel.ThrowException
        };
        /// <summary>
        /// arg[0] = protectCode, arg[1] = protectState, returns CanAlert
        /// </summary>
        public static IEnumerable<TestCaseData> CanAlertCases
        {
            get
            {
                yield return
                    new TestCaseData(
                        /* ScanCode: */ true,
                        /* ScanState */ true).Returns(
                        /* CanAlert */ true);
                yield return
                    new TestCaseData(
                        /* ScanCode: */ false,
                        /* ScanState */ true).Returns(
                        /* CanAlert */ true);
                yield return
                    new TestCaseData(
                        /* ScanCode: */ true,
                        /* ScanState */ false).Returns(
                        /* CanAlert */ true);
                yield return
                    new TestCaseData(
                        /* ScanCode: */ false,
                        /* ScanState */ false).Returns(
                        /* CanAlert */ false);
            }
        }
    }
}