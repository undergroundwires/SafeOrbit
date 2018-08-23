
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
    ///     <p>Abstraction for a factory class that's protected in memory.</p>
    ///     <p>It abstracts strategies for different protection modes</p>
    /// </summary>
    /// <seealso cref="SafeContainerProtectionMode" />
    /// <seealso cref="SafeContainerProtectionMode" />
    public interface ISafeContainer :
        IAlerts,
        IProtectable<SafeContainerProtectionMode>,
        IServiceProvider
   {
        TComponent Get<TComponent>();

        void Register<TComponent>(LifeTime lifeTime = LifeTime.Transient) where TComponent : class, new();

        /// <summary>
        ///     Registers the specified life time.
        /// </summary>
        /// <typeparam name="TComponent">The type of the the component.</typeparam>
        /// <typeparam name="TImplementation">The type of the t implementation.</typeparam>
        /// <param name="lifeTime">The life time.</param>
        void Register<TComponent, TImplementation>(LifeTime lifeTime = LifeTime.Transient)
            where TComponent : class
            where TImplementation : TComponent, new();

        /// <summary>
        ///     Registers the specified instance initializer with an implementation.
        /// </summary>
        /// <typeparam name="TComponent">The type of the the component.</typeparam>
        /// <typeparam name="TImplementation">The type of the t implementation.</typeparam>
        /// <param name="instanceInitializer">The instance initializer.</param>
        /// <param name="lifeTime">The life time.</param>
        void Register<TComponent, TImplementation>(Func<TImplementation> instanceInitializer,
            LifeTime lifeTime = LifeTime.Unknown)
            where TComponent : class
            where TImplementation : TComponent, new();
        /// <summary>
        /// Registers the specified instance initializer.
        /// </summary>
        /// <typeparam name="TComponent">The type of the the component.</typeparam>
        /// <param name="instanceInitializer">The instance initializer.</param>
        /// <param name="lifeTime">The life time.</param>
        void Register<TComponent>(Func<TComponent> instanceInitializer,
            LifeTime lifeTime = LifeTime.Unknown)
            where TComponent : class;
        /// <summary>
        ///     Verifies this instance.
        /// </summary>
        void Verify();
    }
}