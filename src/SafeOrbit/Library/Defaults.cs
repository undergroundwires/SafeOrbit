﻿using SafeOrbit.Library;
using SafeOrbit.Memory;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit
{
    /// <summary>
    ///     Default settings for SafeOrbit classes.
    /// </summary>
    internal class Defaults
    {
        //SafeObject
        public const SafeObjectProtectionMode ObjectProtectionMode = SafeObjectProtectionMode.StateAndCode;

        public static IInitialSafeObjectSettings SafeObjectSettings
            => new InitialSafeObjectSettings(null, false, SafeObjectProtectionMode.StateAndCode, SafeOrbitCore.Current.AlertChannel);


        //SafeContainer
        public const SafeContainerProtectionMode ContainerProtectionMode = SafeContainerProtectionMode.FullProtection;

        //InjectionProtector
        public const InjectionAlertChannel AlertChannel = InjectionAlertChannel.ThrowException;
    }
}