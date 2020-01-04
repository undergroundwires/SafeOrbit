using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Memory.InjectionServices
{
    /// <summary>
    ///     Abstract a service that can alert injections using different channels.
    /// </summary>
    public interface IInjectionAlerter
    {
        /// <summary>
        ///     Alerts the specified information using the specified channel.
        /// </summary>
        /// <param name="info">Injection information.</param>
        /// <param name="channel">Alert channel.</param>
        void Alert(IInjectionMessage info, InjectionAlertChannel channel);
    }
}