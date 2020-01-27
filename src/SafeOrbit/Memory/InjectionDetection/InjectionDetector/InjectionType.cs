namespace SafeOrbit.Memory
{
    public enum InjectionType
    {
        /// <summary>
        ///     Reprents injection when the code of the application is injected.
        /// </summary>
        CodeInjection = 1,

        /// <summary>
        ///     Represents injection when stored values in the memory are injected.
        /// </summary>
        VariableInjection = 2,

        /// <summary>
        ///     Represents injection of the both <see cref="CodeInjection" /> and <see cref="VariableInjection" />
        /// </summary>
        CodeAndVariableInjection = 3
    }
}