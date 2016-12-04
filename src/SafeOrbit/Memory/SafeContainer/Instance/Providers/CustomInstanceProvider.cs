
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
    ///     Provides an instance from a custom <see cref="Func{TResult}" />
    /// </summary>
    /// <seealso cref="SingletonInstanceProvider{TImplementation}"/>
    /// <seealso cref="TransientInstanceProvider{TImplementation}"/>
    /// <seealso cref="IInstanceProvider"/>
    /// <seealso cref="SafeInstanceProviderBase{TImplementation}"/>
    internal class CustomInstanceProvider<TImplementation> : SafeInstanceProviderBase<TImplementation>
    {
        private readonly Func<TImplementation> _instanceGetter;

        public CustomInstanceProvider(Func<TImplementation> instanceGetter, InstanceProtectionMode protectionMode, LifeTime lifeTime = LifeTime.Unknown)
            : base(lifeTime, protectionMode)
        {
            if (instanceGetter == null) throw new ArgumentNullException(nameof(instanceGetter));
            _instanceGetter = instanceGetter;
        }

        /// <summary>
        ///     Internal constructor with all dependencies.
        /// </summary>
        internal CustomInstanceProvider(Func<TImplementation> instanceGetter,
            IInjectionDetector injectionDetector,
            InstanceProtectionMode protectionMode,
            InjectionAlertChannel alertChannel,
            LifeTime lifeTime = LifeTime.Unknown)
            : base(lifeTime, injectionDetector, protectionMode, alertChannel)
        {
            if (instanceGetter == null) throw new ArgumentNullException(nameof(instanceGetter));
            _instanceGetter = instanceGetter;
        }

        /// <summary>
        /// Returns a service object given the specified instance.
        /// </summary>
        /// <returns>TInstanceType.</returns>
        public override TImplementation GetInstance()
        {
            return _instanceGetter.Invoke();
        }

        public override bool CanProtectState => this.LifeTime == LifeTime.Singleton;
    }
}