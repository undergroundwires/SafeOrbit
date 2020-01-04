using System.Collections.Generic;
using NUnit.Framework;

namespace SafeOrbit.Memory
{
    public class SafeObjectCases
    {
        /// <summary>
        ///     Arg[0] = ProtectionMode, Arg[1] = JustState, Arg[1] = JustCode
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
                yield return new TestCaseData(SafeObjectProtectionMode.JustState,
                    SafeObjectProtectionMode.NoProtection);
                yield return new TestCaseData(SafeObjectProtectionMode.StateAndCode,
                    SafeObjectProtectionMode.NoProtection);
                yield return new TestCaseData(SafeObjectProtectionMode.StateAndCode, SafeObjectProtectionMode.JustCode);
            }
        }

        public static IEnumerable<TestCaseData> NonStateProtectionToStateProtectionCases
        {
            get
            {
                yield return new TestCaseData(SafeObjectProtectionMode.NoProtection,
                    SafeObjectProtectionMode.JustState);
                yield return new TestCaseData(SafeObjectProtectionMode.NoProtection,
                    SafeObjectProtectionMode.StateAndCode);
                yield return new TestCaseData(SafeObjectProtectionMode.JustCode, SafeObjectProtectionMode.JustState);
                yield return new TestCaseData(SafeObjectProtectionMode.JustCode, SafeObjectProtectionMode.StateAndCode);
            }
        }
    }
}