using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeOrbit.Memory.SafeContainerServices.Instance
{
    /// <summary>
    /// <see cref="InstanceId"/> property increases by one on each constructor call.
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
        public int InstanceId { get; private set; }
        public bool Equals(InstanceTestClass other) => this.InstanceId == other.InstanceId;
    }

}
