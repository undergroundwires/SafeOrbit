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