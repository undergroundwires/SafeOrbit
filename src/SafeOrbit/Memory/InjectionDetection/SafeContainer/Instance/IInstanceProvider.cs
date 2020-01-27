using System;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Memory.InjectionServices.Protectable;

namespace SafeOrbit.Memory.SafeContainerServices.Instance
{
    internal interface IInstanceProvider :
        IProtectable<InstanceProtectionMode>,
        IAlerts
    {
        Type ImplementationType { get; }
        LifeTime LifeTime { get; }
        object Provide();
    }
}