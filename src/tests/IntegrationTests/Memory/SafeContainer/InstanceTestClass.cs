using System;

namespace SafeOrbit.Memory
{
    public class InstanceTestClass : IEquatable<InstanceTestClass>, IInstanceTestClass
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
        public int InstanceId { get; private set; }
        public bool Equals(InstanceTestClass other) => this.InstanceId == other.InstanceId;
    }
}