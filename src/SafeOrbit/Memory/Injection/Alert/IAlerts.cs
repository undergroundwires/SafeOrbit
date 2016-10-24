namespace SafeOrbit.Memory.InjectionServices
{
    /// <summary>
    ///     Abstracts a class that alerts memory injections.
    /// </summary>
    public interface IAlerts
    {
        /// <summary>
        ///     Gets or sets the alert channel.
        /// </summary>
        /// <value>The alert channel.</value>
        InjectionAlertChannel AlertChannel { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance will alert.
        /// </summary>
        /// <value><c>true</c> if this instance will alert; otherwise, <c>false</c>.</value>
        /// <seealso cref="AlertChannel" />
        bool CanAlert { get; }
    }
}