using System;
using SafeOrbit.Library.Build;
using SafeOrbit.Memory;
using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Library
{
    /// <summary>
    ///     Abstracts the class to access inner library behavior.
    /// </summary>
    public interface ISafeOrbitCore
    {
        /// <summary>
        ///     Safe object container that's being used by the library.
        /// </summary>
        ISafeContainer Factory { get; }

        /// <summary>
        ///     Gets the information regarding to current build of SafeOrbit.
        /// </summary>
        IBuildInfo BuildInfo { get; }

        /// <summary>
        ///     Loads the necessary data early on. For better performance, it's highly recommended to start the
        ///     application early in your application start.
        /// </summary>
        void StartEarly();

        event EventHandler<IInjectionMessage> LibraryInjected;
    }
}