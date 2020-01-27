namespace SafeOrbit.Memory.InjectionServices.Protectable
{
    /// <inheritdoc />
    internal sealed class ProtectionChangeContext<TProtectionLevel> : IProtectionChangeContext<TProtectionLevel>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProtectionChangeContext{TProtectionLevel}" /> class using an old an
        ///     new value.
        /// </summary>
        /// <param name="oldValue">The old value of the protection level.</param>
        /// <param name="newValue">The new value of the protection level.</param>
        public ProtectionChangeContext(TProtectionLevel oldValue, TProtectionLevel newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProtectionChangeContext{TProtectionLevel}" /> class using the new
        ///     value.
        /// </summary>
        /// <param name="newValue">The new value of the protection level.</param>
        public ProtectionChangeContext(TProtectionLevel newValue)
        {
            NewValue = newValue;
        }

        /// <inheritdoc />
        public TProtectionLevel OldValue { get; }

        /// <inheritdoc />
        public TProtectionLevel NewValue { get; }

        /// <inheritdoc />
        public bool IsCanceled { get; set; }
    }
}