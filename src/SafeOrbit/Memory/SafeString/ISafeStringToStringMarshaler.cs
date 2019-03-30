using System;

namespace SafeOrbit.Memory
{
    /// <summary>
    ///     Abstract a marshaler that can convert a <see cref="ISafeString" /> into a <see cref="string" /> until it is
    ///     disposed
    /// </summary>
    /// <seealso cref="IDisposable" />
    /// <seealso cref="ISafeString" />
    /// <seealso cref="SafeString" />
    /// <seealso cref="string" />
    public interface ISafeStringToStringMarshaler : IDisposable
    {
        /// <summary>
        ///     Gets or sets the safe string.
        /// </summary>
        /// <value>The safe string.</value>
        ISafeString SafeString { get; set; }

        /// <summary>
        ///     Gets the disposable string.
        /// </summary>
        /// <value>The disposable string.</value>
        string String { get; }
    }
}