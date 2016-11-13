using System;
using SafeOrbit.Memory;
using SafeOrbit.Memory.Injection;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Library
{
    /// <summary>
    ///     Abstracts the class to access inner library behavior.
    /// </summary>
    public interface ISafeOrbitCore
    {
        ISafeContainer Factory { get; }
        void StartEarly();
        event EventHandler<IInjectionMessage> LibraryInjected;
    }
}