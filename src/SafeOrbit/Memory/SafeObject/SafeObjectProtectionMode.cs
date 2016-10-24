namespace SafeOrbit.Memory
{
    /// <summary>
    ///     Different protection modes for a <see cref="ISafeObject{TObject}" />
    /// </summary>
    /// <seealso cref="ISafeObject{TObject}" />
    public enum SafeObjectProtectionMode
    {
        /// <summary>
        ///     Provides the maximum security by protecting the object against both state and code injections
        /// </summary>
        StateAndCode = 3,

        /// <summary>
        ///     Provides protection against only state injections. This option should be selected if the code of the object is
        ///     designed to change its  code dynamically. The type will be vulnerable to code injections.
        /// </summary>
        JustState = 2,

        /// <summary>
        ///     Provides protection against only code injections. This option might be good if the object is a stateless class, or
        ///     the data it's holding is not sensitive.
        /// </summary>
        JustCode = 1,

        /// <summary>
        ///     Provides no protection / security.
        ///     <b>Important : This option is completely un-safe and will disable injection scans.</b>
        /// </summary>
        NoProtection = 0
    }
}