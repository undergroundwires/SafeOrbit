using System;
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
        public static IInjectionAlerter StaticInstance = new InjectionAlerter(
            new RaiseEventAlerter(), new ThrowExceptionAlerter(), new DebugFailAlerter(), new DebugWriteAlerter());

        private readonly IAlerter _raiseEventAlerter;
        private readonly IAlerter _throwExceptionAlerter;
        private readonly IAlerter _debugFailAlerter;
        private readonly IAlerter _debugWriteAlerter;

        internal InjectionAlerter(IAlerter raiseEventAlerter, IAlerter throwExceptionAlerter, IAlerter debugFailAlerter,
            IAlerter debugWriteAlerter)
        {
            if (raiseEventAlerter == null) throw new ArgumentNullException(nameof(raiseEventAlerter));
            if (throwExceptionAlerter == null) throw new ArgumentNullException(nameof(throwExceptionAlerter));
            if (debugFailAlerter == null) throw new ArgumentNullException(nameof(debugFailAlerter));
            _raiseEventAlerter = raiseEventAlerter;
            _throwExceptionAlerter = throwExceptionAlerter;
            _debugFailAlerter = debugFailAlerter;
            _debugWriteAlerter = debugWriteAlerter;
        }

        public void Alert(IInjectionMessage info, InjectionAlertChannel channel)
        {
            switch (channel)
            {
                case InjectionAlertChannel.RaiseEvent:
                    _raiseEventAlerter.Alert(info);
                    break;
                case InjectionAlertChannel.ThrowException:
                    _throwExceptionAlerter.Alert(info);
                    break;
                case InjectionAlertChannel.DebugFail:
                    _debugFailAlerter.Alert(info);
                    break;
                case InjectionAlertChannel.DebugWrite:
                    _debugWriteAlerter.Alert(info);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(channel), channel, null);
            }
        }
    }
}