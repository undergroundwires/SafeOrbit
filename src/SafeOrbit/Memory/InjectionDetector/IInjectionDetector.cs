using System;
using System.Collections.Generic;
using SafeOrbit.Exceptions;
using SafeOrbit.Memory.Injection;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory
{
    /// <summary>
    ///     <see cref="IInjectionDetector" /> abstracts a service that keeps track of C# objects and classes. It alert .
    /// </summary>
    /// <seealso cref="AlertUnnotifiedChanges"/>
    /// <seealso cref="IAlerts"/>
    /// <remarks>
    ///     <p>The work-method is simple :</p>
    ///     <p>1. User validates last changes made by application using <see cref="NotifyChanges" /> methods.</p>
    ///     <p>
    ///         2. The class throws <see cref="MemoryInjectionException" /> on <see cref="AlertUnnotifiedChanges" /> if the object has
    ///         been changed without validating.
    ///     </p>
    /// </remarks>
    /// <seealso cref="MemoryInjectionException" />
    /// <seealso cref="InjectionDetector" />
    /// <seealso cref="IAlerts" />
    public interface IInjectionDetector :
        IDisposable,
        IAlerts
    {
        bool ScanState { get; set; }
        bool ScanCode { get; set; }
        /// <summary>
        ///     Validates object depending on <see cref="ScanState" /> and <see cref="ScanCode" />.
        /// </summary>
        /// <remarks>
        ///     <p>The object must be validated by application using <see cref="NotifyChanges" /> before calling this method.</p>
        /// </remarks>
        /// <param name="object">Object that this instance has been notified by <see cref="NotifyChanges"/></param>
        /// <exception cref="MemoryInjectionException">
        ///     If the state or code of the object has been changed without
        ///     <see cref="NotifyChanges" /> method being called.
        /// </exception>
        void AlertUnnotifiedChanges(object @object);
        /// <summary>
        /// Verifies the latest changes to the object.
        /// </summary>
        /// <param name="object">Object that this instance scans/tracks.</param>
        void NotifyChanges(object @object);
    }
}