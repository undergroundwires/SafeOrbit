using System;
using SafeOrbit.Memory;
using SafeOrbit.Memory.SafeBytesServices.Factory;

namespace SafeOrbit.Library.StartEarly
{
    /// <summary>
    ///     Initializes <see cref="ISafeByteFactory" /> instance from <see cref="ISafeContainer" />.
    /// </summary>
    /// <seealso cref="StartEarlyTaskBase" />
    /// <seealso cref="MemoryCachedSafeByteFactory" />
    internal class SafeByteFactoryInitializer : StartEarlyTaskBase
    {
        private readonly ISafeContainer _container;

        public SafeByteFactoryInitializer(ISafeContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public override void Prepare()
        {
            _container.Get<ISafeByteFactory>().Initialize();
        }
    }
}