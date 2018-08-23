
/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

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
        /// </summary>
        /// <seealso cref="IsReadOnly" />
        /// <param name="modification">The modification.</param>
        void ApplyChanges(Action<TObject> modification);
    }
}