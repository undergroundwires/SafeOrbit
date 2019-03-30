﻿using System.Diagnostics;
using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Memory.InjectionServices.Alerters
{
    internal class DebugWriteAlerter : IAlerter
    {
        public static IAlerter GetInstance() => new DebugWriteAlerter();
        public InjectionAlertChannel Channel { get; } = InjectionAlertChannel.DebugWrite;

        public void Alert(IInjectionMessage info)
        {
            Debug.Write($"An object has been infected by {info.InjectionType} at {info.InjectionDetectionTime.ToLocalTime()}");
            Debug.Write(info);
        }
    }
}