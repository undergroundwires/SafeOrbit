namespace SafeOrbit.Core.Protectable
{
    /// <summary>
    ///     The event arguments when the protection level switch is requested.
    /// </summary>
    /// <typeparam name="TProtectionLevel">The type of the protection level.</typeparam>
    public interface IProtectionChangeContext<out TProtectionLevel>
    {
        /// <summary>
        ///     Gets the old value of the <typeparamref name="TProtectionLevel" />.
        /// </summary>
        /// <value>The old value of the <typeparamref name="TProtectionLevel" />.</value>
        TProtectionLevel OldValue { get; }

        /// <summary>
        ///     Gets the new value of the <typeparamref name="TProtectionLevel" />. This is the value that's requested to be set.
        /// </summary>
        /// <value>The new value of the <typeparamref name="TProtectionLevel" /> that's requested to be set.</value>
        TProtectionLevel NewValue { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the protection level switching is canceled.
        /// </summary>
        /// <value><c>true</c> if this protection level switching is canceled; otherwise, <c>false</c>.</value>
        bool IsCanceled { get; set; }
    }
}