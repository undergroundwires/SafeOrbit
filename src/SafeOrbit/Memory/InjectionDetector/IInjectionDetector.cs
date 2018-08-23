
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

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