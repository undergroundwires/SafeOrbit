using System;
using SafeOrbit.Library;
using SafeOrbit.Memory.Injection;
using SafeOrbit.Memory.InjectionServices.Alerters;

namespace SafeOrbit.Memory.InjectionServices
{
    /// <summary>
    /// Alerts an <see cref="IInjectionMessage"/> using the right <see cref="IAlerter"/> instance.
    /// </summary>
    /// <seealso cref="IInjectionAlerter" />
    /// <seealso cref="IAlerter"/>
    internal class InjectionAlerter : IInjectionAlerter
    {
        private readonly IAlerterFactory _alerterFactory;

        public static readonly IInjectionAlerter StaticInstance = new InjectionAlerter(new AlerterFactory());
        internal InjectionAlerter(IAlerterFactory alerterFactory)
        {
            _alerterFactory = alerterFactory ?? throw new ArgumentNullException(nameof(alerterFactory));
        }

        public void Alert(IInjectionMessage info, InjectionAlertChannel channel)
        {
            var alerter = _alerterFactory.Get(channel);
            alerter.Alert(info);
        }
    }
}