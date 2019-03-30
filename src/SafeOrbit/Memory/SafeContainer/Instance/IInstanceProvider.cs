using System;
using SafeOrbit.Infrastructure.Protectable;
using SafeOrbit.Memory.InjectionServices;

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