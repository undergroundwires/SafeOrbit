using SafeOrbit.Library;
using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Memory.InjectionServices.Alerters
{
    /// <summary>
    ///     Raises <see cref="LibraryManagement.LibraryInjected"/> event.
    /// </summary>
    internal class RaiseEventAlerter : IAlerter
    {
        public InjectionAlertChannel Channel { get; } = InjectionAlertChannel.RaiseEvent;

        public void Alert(IInjectionMessage info)
        {
            LibraryManagement.LibraryInjected?.Invoke(info.InjectedObject, info);
        }
    }
}