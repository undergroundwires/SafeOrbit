using SafeOrbit.Memory.InjectionServices.Alerters;

namespace SafeOrbit.Memory.InjectionServices
{
    internal interface IAlerterFactory
    {
        IAlerter Get(InjectionAlertChannel channel);
    }
}