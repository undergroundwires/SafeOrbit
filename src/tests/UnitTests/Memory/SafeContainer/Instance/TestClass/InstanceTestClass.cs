using System;

namespace SafeOrbit.Memory.SafeContainerServices.Instance
{
    /// <summary>
    ///     <see cref="InstanceId" /> property increases by one on each constructor call.
    /// </summary>
    public class InstanceTestClass : IInstanceTestClass, IEquatable<InstanceTestClass>
    {
        private static int _instanceCounter;

        public InstanceTestClass()
        {
            InstanceId = _instanceCounter;
            _instanceCounter++;
        }

        public InstanceTestClass(int instanceId)
        {
            InstanceId = instanceId;
        }

        public int InstanceId { get; }
        public bool Equals(InstanceTestClass other) => InstanceId == other.InstanceId;
    }
}