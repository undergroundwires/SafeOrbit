using SafeOrbit.Library;
using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Memory.InjectionServices.Alerters
{
    /// <summary>
    ///     Raises an event.
    /// </summary>
    internal class RaiseEventAlerter : IAlerter
    {
        private readonly SafeOrbitCore _safeOrbitCore;

        public RaiseEventAlerter(SafeOrbitCore safeOrbitCore)
        {
            _safeOrbitCore = safeOrbitCore;
        }

        public InjectionAlertChannel Channel { get; } = InjectionAlertChannel.RaiseEvent;

        public void Alert(IInjectionMessage info)
        {
            _safeOrbitCore.AlertInjection(info.InjectedObject, info);
        }
    }
}