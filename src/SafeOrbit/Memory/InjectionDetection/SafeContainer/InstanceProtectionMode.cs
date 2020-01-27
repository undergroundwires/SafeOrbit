namespace SafeOrbit.Memory
{
    /// <summary>
    ///     Different protection modes for a <see cref="ISafeObject{TObject}" />
    /// </summary>
    /// <seealso cref="ISafeObject{TObject}" />
    public enum InstanceProtectionMode
    {
        /// <summary>
        ///     Provides the maximum security by protecting the instance against both state and code injections
        /// </summary>
        StateAndCode = 1,

        /// <summary>
        ///     Provides protection against only state injections. This option should be selected if the code of the the instance
        ///     is designed to change its code dynamically. The type will be vulnerable to code injections.
        /// </summary>
        JustState = 3,

        /// <summary>
        ///     Provides protection against only code injections. This option might be good if the instance type is a stateless
        ///     class, or the data it's holding is not sensitive.
        /// </summary>
        JustCode = 2,


        /// <summary>
        ///     Provides no protection / security.
        /// </summary>
        NoProtection = 0
    }
}