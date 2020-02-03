using System;

namespace SafeOrbit.Memory.SafeStringServices
{
    public interface IDisposableString : IDisposable
    {
        /// <summary>
        ///     Gets the disposable string.
        /// </summary>
        /// <value>The disposable string.</value>
        string String { get; }
    }
}
