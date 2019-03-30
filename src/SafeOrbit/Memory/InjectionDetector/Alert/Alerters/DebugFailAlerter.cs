using System.Diagnostics;
using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Memory.InjectionServices.Alerters
{
    internal class DebugFailAlerter : IAlerter
    {
        public InjectionAlertChannel Channel { get; } = InjectionAlertChannel.DebugFail;

        public void Alert(IInjectionMessage info)
        {
            Debug.Fail($"An object has been infected by {info.InjectionType} at {info.InjectionDetectionTime.ToLocalTime()}");
        }
    }
}