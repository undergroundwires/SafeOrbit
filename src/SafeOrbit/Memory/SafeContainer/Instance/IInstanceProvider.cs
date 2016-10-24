using System;
using System.Diagnostics;
using SafeOrbit.Memory.Common;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory.SafeContainerServices.Instance
{
    internal interface IInstanceProvider :
        IProtectionLevelSwitchProvider<InstanceProtectionMode>,
        IAlerts        
    {
        Type ImplementationType { get; }
        LifeTime LifeTime { get; }
        object Provide();
    }
}