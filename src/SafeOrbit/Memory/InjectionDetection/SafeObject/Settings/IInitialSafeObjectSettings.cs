using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory
{
    /// <summary>
    ///     Abstraction of a class that holds some values for initial settings of <see cref="ISafeObject{TObject}" />
    /// </summary>
    /// <seealso cref="ISafeObject{TObject}" />
    public interface IInitialSafeObjectSettings
    {
        /// <summary>
        ///     Gets or sets the initial instance for <see cref="ISafeObject{TObject}" />
        /// </summary>
        /// <seealso cref="ISafeObject{TObject}" />
        /// <seealso cref="SafeObjectProtectionMode" />
        object InitialValue { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the requested <see cref="ISafeObject{TObject}" />  instance is modifiable
        ///     after its created.
        /// </summary>
        /// <seealso cref="ISafeObject{TObject}" />
        /// <seealso cref="SafeObjectProtectionMode" />
        /// <value><c>true</c> if this instance is read only; if modifiable, <c>false</c>.</value>
        bool IsReadOnly { get; }

        /// <summary>
        ///     Gets or sets the initial protection mode of the <see cref="ISafeObject{TObject}" /> instance.
        /// </summary>
        /// <seealso cref="ISafeObject{TObject}" />
        /// <seealso cref="SafeObjectProtectionMode" />
        /// <value>The protection mode.</value>
        SafeObjectProtectionMode ProtectionMode { get; }

        /// <summary>
        ///     Preferred way to notify injections.
        /// </summary>
        InjectionAlertChannel AlertChannel { get; }
    }
}