using System;
using SafeOrbit.Exceptions;
using SafeOrbit.Memory.Common;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory
{
    /// <seealso cref="SafeObject{TObject}" />
    /// <seealso cref="SafeObjectProtectionMode" />
    public interface ISafeObject<out TObject> :
        IProtectionLevelSwitchProvider<SafeObjectProtectionMode>,
        IAlerts,
        IDisposable
        where TObject : class
    {
        /// <summary>
        /// Gets a value indicating whether this instance is modifiable.
        /// </summary>
        /// <value><c>true</c> if this instance is modifiable; otherwise, <c>false</c>.</value>
        /// <seealso cref="MakeReadOnly"/>
        bool IsReadOnly { get; }
        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <value>The object.</value>
        TObject Object { get; }
        /// <summary>
        /// Closes this instance to any kind of changes.
        /// </summary>
        /// <seealso cref="IsReadOnly"/>
        void MakeReadOnly();
        /// <summary>
        /// Verifies the last changes on the object.
        /// </summary>
        /// <exception cref="ReadOnlyAccessForbiddenException">This instance of <see cref="TObject"/> is marked as ReadOnly.</exception>
        /// <seealso cref="IsReadOnly"/>
        /// <param name="modification">The modification.</param>
        void ApplyChanges(Action<TObject> modification);
    }
}