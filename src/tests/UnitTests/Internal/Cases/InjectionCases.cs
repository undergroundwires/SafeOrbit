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
        ///     arg[0] = protectCode, arg[1] = protectState, returns CanAlert
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