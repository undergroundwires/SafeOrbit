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