using System;
using SafeOrbit.Library;

namespace SafeOrbit.Memory
{
    /// <summary>
    ///     <see cref="IFactory{TInstance}" /> implementation, using <see cref="ISafeContainer" />.
    /// </summary>
    /// <typeparam name="TComponent">The type of the component.</typeparam>
    /// <seealso cref="IFactory{TComponent}" />
    internal class SafeContainerWrapper<TComponent> : IFactory<TComponent>
    {
        private readonly ISafeContainer _safeContainer;

        public SafeContainerWrapper() : this(SafeOrbitCore.Current.Factory)
        {
        }

        public SafeContainerWrapper(ISafeContainer safeContainer)
        {
            _safeContainer = safeContainer ?? throw new ArgumentNullException(nameof(safeContainer));
        }

        public TComponent Create()
        {
            return _safeContainer.Get<TComponent>();
        }

        public static IFactory<TComponent> Wrap() => new SafeContainerWrapper<TComponent>();
    }
}