
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

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
using System.ComponentModel;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory
{
    /// <summary>
    ///     Initial settings for <see cref="SafeObject{TObject}" /> instances
    /// </summary>
    /// <seealso cref="SafeObject{TObject}" />
    public class InitialSafeObjectSettings : IInitialSafeObjectSettings
    {
        /// <summary>
        ///     Gets or sets the initial instance for <see cref="SafeObject{TObject}" />
        /// </summary>
        /// <seealso cref="SafeObject{TObject}" />
        /// <seealso cref="SafeObjectProtectionMode" />
        public object InitialValue { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the requested <see cref="SafeObject{TObject}" />  instance is modifiable
        ///     after its created.
        /// </summary>
        /// <seealso cref="SafeObject{TObject}" />
        /// <seealso cref="SafeObjectProtectionMode" />
        public bool IsReadOnly { get; set; }

        /// <summary>
        ///     Gets or sets the initial protection mode of the <see cref="SafeObject{TObject}" /> instance.
        /// </summary>
        /// <seealso cref="SafeObject{TObject}" />
        /// <seealso cref="SafeObjectProtectionMode" />
        /// <value>The protection mode.</value>
        public SafeObjectProtectionMode ProtectionMode { get; set; }

        public InjectionAlertChannel AlertChannel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitialSafeObjectSettings"/> class.
        /// </summary>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="isReadOnly">if set to <c>false</c> the instance will be modifiable initially.</param>
        /// <param name="protectionMode">The initial protection mode.</param>
        /// <param name="alertChannel">Gets/sets the alert channel for the inner <see cref="IInjectionDetector"/></param>
        public InitialSafeObjectSettings(object initialValue, bool isReadOnly, SafeObjectProtectionMode protectionMode, InjectionAlertChannel alertChannel)
        {
            InitialValue = initialValue;
            IsReadOnly = isReadOnly;
            ProtectionMode = protectionMode;
            AlertChannel = alertChannel;
        }
    }
}