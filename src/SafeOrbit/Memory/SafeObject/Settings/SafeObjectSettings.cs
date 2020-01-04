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
        ///     Initializes a new instance of the <see cref="InitialSafeObjectSettings" /> class.
        /// </summary>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="isReadOnly">if set to <c>false</c> the instance will be modifiable initially.</param>
        /// <param name="protectionMode">The initial protection mode.</param>
        /// <param name="alertChannel">Sets the alert channel for the inner <see cref="IInjectionDetector" /></param>
        public InitialSafeObjectSettings(object initialValue, bool isReadOnly, SafeObjectProtectionMode protectionMode,
            InjectionAlertChannel alertChannel)
        {
            InitialValue = initialValue;
            IsReadOnly = isReadOnly;
            ProtectionMode = protectionMode;
            AlertChannel = alertChannel;
        }

        /// <inheritdoc />
        /// <seealso cref="SafeObject{TObject}" />
        public object InitialValue { get; set; }

        /// <inheritdoc />
        /// <seealso cref="SafeObject{TObject}" />
        public bool IsReadOnly { get; set; }

        /// <inheritdoc />
        /// <seealso cref="SafeObject{TObject}" />
        public SafeObjectProtectionMode ProtectionMode { get; set; }

        /// <inheritdoc />
        public InjectionAlertChannel AlertChannel { get; }
    }
}