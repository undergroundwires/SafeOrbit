
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

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
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Providers
{
    /// <summary>
    ///     An abstraction for a factory class to retrieve right <seealso cref="IInstanceProvider" /> instance for right
    ///     <see cref="LifeTime" /> and <see cref="SafeContainerProtectionMode" />.
    /// </summary>
    /// <seealso cref="IInstanceProvider" />
    /// <seealso cref="LifeTime" />
    /// <seealso cref="SafeContainerProtectionMode" />
    internal interface IInstanceProviderFactory
    {
        /// <summary>
        ///     Provides an <see cref="IInstanceProvider" /> for the specified <see cref="LifeTime" />
        /// </summary>
        /// <remarks>
        ///     <p>
        ///         Only accepted <see cref="LifeTime" />'s are <see cref="LifeTime.Transient" /> and
        ///         <see cref="LifeTime.Singleton" />.
        ///     </p>
        /// </remarks>
        /// <typeparam name="TImplementation">Type of the requested instance.</typeparam>
        /// <param name="lifeTime">The life time.</param>
        /// <param name="protectionMode">Initial protection mode for the result instance.</param>
        /// <param name="alertChannel">Initial alert channel for the result instance.</param>
        /// <returns>A <see cref="IInstanceProvider" />.</returns>
        /// <seealso cref="IInstanceProvider" />
        /// <seealso cref="InstanceProtectionMode" />
        /// <seealso cref="InjectionAlertChannel" />
        /// <seealso cref="LifeTime" />
        IInstanceProvider Get<TImplementation>(LifeTime lifeTime, InstanceProtectionMode protectionMode, InjectionAlertChannel alertChannel) where TImplementation : new();

        /// <summary>
        ///     Provides an <see cref="IInstanceProvider" /> with a specified instance getter function.
        /// </summary>
        IInstanceProvider Get<TImplementation>(Func<TImplementation> instanceGetter, InstanceProtectionMode protectionMode, InjectionAlertChannel alertChannel, LifeTime lifeTime = LifeTime.Unknown);
    }
}