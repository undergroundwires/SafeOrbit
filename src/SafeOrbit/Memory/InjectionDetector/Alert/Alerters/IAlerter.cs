using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Memory.InjectionServices.Alerters
{
    internal interface IAlerter
    {
        InjectionAlertChannel Channel { get; }
        void Alert(IInjectionMessage info);
    }
}