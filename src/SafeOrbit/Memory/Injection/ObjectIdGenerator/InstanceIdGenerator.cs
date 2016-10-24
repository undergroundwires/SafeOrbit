using System;
using System.Runtime.Serialization;

namespace SafeOrbit.Memory.InjectionServices
{
    /// <summary>
    /// <p>Generates an ID for each instance of any <see cref="object"/>.</p>
    /// <p>A wrapper around <see cref="ObjectIDGenerator"/></p>
    /// </summary>
    /// <seealso cref="IObjectIdGenerator" />
    public class InstanceIdGenerator : IObjectIdGenerator
    {
        public static readonly IObjectIdGenerator StaticInstance = new InstanceIdGenerator();
        private static readonly Lazy<ObjectIDGenerator> IdGeneratorLazy = new Lazy<ObjectIDGenerator>();
        public long GetStateId(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            bool firstTime;
            var result = IdGeneratorLazy.Value.GetId(obj, out firstTime);
            return result;
        }
    }
}