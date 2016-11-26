
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

namespace SafeOrbit.Memory
{
    public class SafeObjectCases
    {
        /// <summary>
        /// Arg[0] = ProtectionMode, Arg[1] = JustState, Arg[1] = JustCode
        /// </summary>
        public static IEnumerable<TestCaseData> ProtectionModeAndProtectVariables
        {
            get
            {
                yield return new TestCaseData(SafeObjectProtectionMode.StateAndCode, true, true);
                yield return new TestCaseData(SafeObjectProtectionMode.JustState, true, false);
                yield return new TestCaseData(SafeObjectProtectionMode.JustCode, false, true);
                yield return new TestCaseData(SafeObjectProtectionMode.NoProtection, false, false);
            }
        }

        public static SafeObjectProtectionMode[] SafeObjectProtectionModeCases =
        {
            SafeObjectProtectionMode.JustCode, SafeObjectProtectionMode.JustState,
            SafeObjectProtectionMode.NoProtection, SafeObjectProtectionMode.StateAndCode
        };

        public static SafeObjectProtectionMode[] AlertingProtectionModes =
        {
            SafeObjectProtectionMode.JustCode, SafeObjectProtectionMode.JustState, SafeObjectProtectionMode.StateAndCode
        };

        public static IEnumerable<TestCaseData> StateProtectionToNonStateProtectionCases
        {
            get
            {
                yield return new TestCaseData(SafeObjectProtectionMode.JustState, SafeObjectProtectionMode.NoProtection);
                yield return new TestCaseData(SafeObjectProtectionMode.StateAndCode, SafeObjectProtectionMode.NoProtection);
                yield return new TestCaseData(SafeObjectProtectionMode.StateAndCode, SafeObjectProtectionMode.JustCode);
            }
        }
        public static IEnumerable<TestCaseData> NonStateProtectionToStateProtectionCases
        {
            get
            {
                yield return new TestCaseData(SafeObjectProtectionMode.NoProtection, SafeObjectProtectionMode.JustState);
                yield return new TestCaseData(SafeObjectProtectionMode.NoProtection, SafeObjectProtectionMode.StateAndCode);
                yield return new TestCaseData(SafeObjectProtectionMode.JustCode, SafeObjectProtectionMode.JustState);
                yield return new TestCaseData(SafeObjectProtectionMode.JustCode, SafeObjectProtectionMode.StateAndCode);
            }
        }

    }
}