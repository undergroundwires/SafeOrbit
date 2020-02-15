using System.Diagnostics;
using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Memory.InjectionServices.Alerters
{
    internal class DebugWriteAlerter : IAlerter
    {
        public InjectionAlertChannel Channel { get; } = InjectionAlertChannel.DebugWrite;

        public void Alert(IInjectionMessage info)
        {
            Debug.Write(info.ToString());
        }

        public static IAlerter GetInstance() => new DebugWriteAlerter();
    }
}