using System;
using SafeOrbit.Memory;
using SafeOrbit.Memory.Injection;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Library
{
    public interface ISafeOrbitCore
    {
        InjectionAlertChannel AlertChannel { get; set; }
        ISafeContainer Factory { get; }
        void StartEarly();
        event EventHandler<IInjectionMessage> LibraryInjected;
    }
}