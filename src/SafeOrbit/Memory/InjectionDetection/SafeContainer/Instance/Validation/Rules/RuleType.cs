namespace SafeOrbit.Memory.SafeContainerServices.Instance.Validation
{
    internal enum RuleType
    {
        /// <summary>
        ///     If the rule is not satisfied the factory can continue.
        /// </summary>
        Warning,

        /// <summary>
        ///     If the rules is not satisfied then the factory should throw.
        /// </summary>
        Error
    }
}