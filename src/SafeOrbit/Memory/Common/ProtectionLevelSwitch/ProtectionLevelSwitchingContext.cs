namespace SafeOrbit.Memory.Common
{
    /// <summary>
    /// A basic class that implements <see cref="IProtectionLevelSwitchingContext{TProtectionLevel}"/>. This class cannot be inherited.
    /// </summary>
    /// <typeparam name="TProtectionLevel">The type of the protection level.</typeparam>
    /// <seealso cref="IProtectionLevelSwitchingContext{TProtectionLevel}" />
    internal sealed class ProtectionLevelSwitchingContext<TProtectionLevel> : IProtectionLevelSwitchingContext<TProtectionLevel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectionLevelSwitchingContext{TProtectionLevel}"/> class using an old an new value.
        /// </summary>
        /// <param name="oldValue">The old value of the protection level.</param>
        /// <param name="newValue">The new value of the protection level.</param>
        public ProtectionLevelSwitchingContext(TProtectionLevel oldValue, TProtectionLevel newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectionLevelSwitchingContext{TProtectionLevel}"/> class using the new value.
        /// </summary>
        /// <param name="newValue">The new value of the protection level.</param>
        public ProtectionLevelSwitchingContext(TProtectionLevel newValue)
        {
            NewValue = newValue;
        }

        /// <summary>
        /// Gets the old value of the <see cref="TProtectionLevel"/>.
        /// </summary>
        /// <value>The old value of the <see cref="TProtectionLevel"/>.</value>
        public TProtectionLevel OldValue { get; }
        /// <summary>
        /// Gets the new value of the <see cref="TProtectionLevel"/>. This is the value that's requested to be set.
        /// </summary>
        /// <value>The new value of the <see cref="TProtectionLevel"/> that's requested to be set.</value>
        public TProtectionLevel NewValue { get; }
        /// <summary>
        /// Gets or sets a value indicating whether the protection level switching is canceled.
        /// </summary>
        /// <value><c>true</c> if this protection level switching is canceled; otherwise, <c>false</c>.</value>
        public bool IsCanceled { get; set; }
    }
}