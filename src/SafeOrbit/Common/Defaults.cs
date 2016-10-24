using SafeOrbit.Memory;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit
{
    internal class Defaults
    {
        public const InjectionAlertChannel AlertChannel = InjectionAlertChannel.ThrowException;
        public const SafeContainerProtectionMode ContainerProtectionMode = SafeContainerProtectionMode.FullProtection;
        public const SafeObjectProtectionMode ObjectProtectionMode = SafeObjectProtectionMode.StateAndCode;
        public static IInitialSafeObjectSettings SafeObjectSettings => new InitialSafeObjectSettings(null, false, SafeObjectProtectionMode.StateAndCode, AlertChannel);
    }
}