namespace SafeOrbit.Infrastructure.Protectable
{
    /// <summary>
    ///     A basic class that implements <see cref="IProtectionChangeContext{TProtectionLevel}" />. This class cannot be
    ///     inherited.
    /// </summary>
    /// <typeparam name="TProtectionLevel">The type of the protection level.</typeparam>
    /// <seealso cref="IProtectionChangeContext{TProtectionLevel}" />
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

        /// <summary>
        ///     Gets the old value of the <typeparamref name="TProtectionLevel" />.
        /// </summary>
        /// <value>The old value of the <typeparamref name="TProtectionLevel" />.</value>
        public TProtectionLevel OldValue { get; }

        /// <summary>
        ///     Gets the new value of the <typeparamref name="TProtectionLevel" />. This is the value that's requested to be set.
        /// </summary>
        /// <value>The new value of the <typeparamref name="TProtectionLevel" /> that's requested to be set.</value>
        public TProtectionLevel NewValue { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the protection level switching is canceled.
        /// </summary>
        /// <value><c>true</c> if this protection level switching is canceled; otherwise, <c>false</c>.</value>
        public bool IsCanceled { get; set; }
    }
}