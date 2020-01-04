using System.Collections.Generic;
using NUnit.Framework;

namespace SafeOrbit.Memory
{
    public class SafeContainerCases
    {
        public static IEnumerable<SafeContainerProtectionMode> ProtectionModes = new[]
            {SafeContainerProtectionMode.NonProtection, SafeContainerProtectionMode.FullProtection};

        public static IEnumerable<TestCaseData> SafeContainerProtectionMode_To_InstanceProtectionMode_Vector
        {
            get
            {
                yield return new TestCaseData(SafeContainerProtectionMode.FullProtection,
                    InstanceProtectionMode.StateAndCode);
                yield return new TestCaseData(SafeContainerProtectionMode.NonProtection,
                    InstanceProtectionMode.NoProtection);
            }
        }

        public static IEnumerable<TestCaseData> SafeContainerProtectionMode_To_SafeObjectProtectionMode_Vector
        {
            get
            {
                yield return new TestCaseData(SafeContainerProtectionMode.FullProtection,
                    SafeObjectProtectionMode.JustState);
                yield return new TestCaseData(SafeContainerProtectionMode.NonProtection,
                    SafeObjectProtectionMode.NoProtection);
            }
        }
    }
}