using System;
using SafeOrbit.Memory;
using SafeOrbit.Memory.Injection;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Library
{
    public interface ILibraryManagement
    {
        InjectionAlertChannel AlertChannel { get; set; }
        ISafeContainer Factory { get; }
        SafeContainerProtectionMode ProtectionMode { get; set; }
        void StartEarly(SafeContainerProtectionMode protectionMode = SafeContainerProtectionMode.NonProtection);
        event EventHandler<IInjectionMessage> LibraryInjected;
    }
}