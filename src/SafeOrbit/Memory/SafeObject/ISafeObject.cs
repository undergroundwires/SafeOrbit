using System;
using SafeOrbit.Infrastructure.Protectable;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory
{
    /// <summary>
    ///     Abstracts an object that can detect the injections to itself.
    /// </summary>
    /// <typeparam name="TObject">The type of the the class.</typeparam>
    /// <seealso cref="IProtectable{TProtectionLevel}" />
    /// <seealso cref="IAlerts" />
    /// <seealso cref="IDisposable" />
    /// <seealso cref="SafeObjectProtectionMode" />
    public interface ISafeObject<out TObject> :
            IProtectable<SafeObjectProtectionMode>,
            IAlerts,
            IDisposable
        where TObject : class
    {
        /// <summary>
        ///     Gets a value indicating whether this instance is modifiable.
        /// </summary>
        /// <value><c>true</c> if this instance is modifiable; otherwise, <c>false</c>.</value>
        /// <seealso cref="MakeReadOnly" />
        bool IsReadOnly { get; }

        /// <summary>
        ///     Gets the object.
        /// </summary>
        /// <value>The object.</value>
        TObject Object { get; }

        /// <summary>
        ///     Closes this instance to any kind of changes.
        /// </summary>
        /// <seealso cref="IsReadOnly" />
        void MakeReadOnly();

        /// <summary>
        ///     Verifies the last changes on the object.
        ///     The object should only be modified with this method to authorize the modification.
        /// </summary>
        /// <seealso cref="IsReadOnly" />
        /// <param name="modification">The modification.</param>
        void ApplyChanges(Action<TObject> modification);
    }
}