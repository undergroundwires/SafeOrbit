
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

using System.Collections.Generic;
using NUnit.Framework;

namespace SafeOrbit.Memory
{
    public class InstanceCases
    {
        public static readonly LifeTime[] LifeTimes =
        {
            LifeTime.Transient, LifeTime.Unknown, LifeTime.Singleton
        };

        public static readonly InstanceProtectionMode[] InstanceProtectionModes =
        {
            InstanceProtectionMode.JustCode, InstanceProtectionMode.JustState,
            InstanceProtectionMode.NoProtection, InstanceProtectionMode.StateAndCode
        };

        public static readonly InstanceProtectionMode[] AlertingInstanceProtectionModes =
        {
            InstanceProtectionMode.JustCode, InstanceProtectionMode.JustState, InstanceProtectionMode.StateAndCode
        };

        /// <summary>
        /// Arg[0] = ProtectionMode, Arg[1] = JustState, Arg[1] = JustCode
        /// </summary>
        public static IEnumerable<TestCaseData> ProtectionModeAndProtectVariables
        {
            get
            {
                yield return new TestCaseData(InstanceProtectionMode.StateAndCode, true, true);
                yield return new TestCaseData(InstanceProtectionMode.JustState, true, false);
                yield return new TestCaseData(InstanceProtectionMode.JustCode, false, true);
                yield return new TestCaseData(InstanceProtectionMode.NoProtection, false, false);
            }
        }

        public static IEnumerable<TestCaseData> StateProtection_To_NonStateProtection
        {
            get
            {
                yield return new TestCaseData(InstanceProtectionMode.JustState, InstanceProtectionMode.JustCode);
                yield return new TestCaseData(InstanceProtectionMode.JustState, InstanceProtectionMode.NoProtection);
                yield return new TestCaseData(InstanceProtectionMode.StateAndCode, InstanceProtectionMode.NoProtection);
                yield return new TestCaseData(InstanceProtectionMode.StateAndCode, InstanceProtectionMode.JustCode);
                yield return new TestCaseData(InstanceProtectionMode.StateAndCode, InstanceProtectionMode.JustState);
            }
        }
        public static IEnumerable<TestCaseData> NonStateProtection_To_StateProtection
        {
            get
            {
                yield return new TestCaseData(InstanceProtectionMode.NoProtection, InstanceProtectionMode.JustState);
                yield return new TestCaseData(InstanceProtectionMode.NoProtection, InstanceProtectionMode.StateAndCode);
                yield return new TestCaseData(InstanceProtectionMode.JustCode, InstanceProtectionMode.JustState);
                yield return new TestCaseData(InstanceProtectionMode.JustCode, InstanceProtectionMode.StateAndCode);
            }
        }
    }
}