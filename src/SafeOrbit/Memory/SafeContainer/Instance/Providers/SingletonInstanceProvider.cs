
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

using System.Diagnostics;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Providers
{
    /// <summary>
    ///     Provides the same instance of <typeparamref name="TImplementation" />.
    /// </summary>
    /// <see cref="LifeTime.Singleton"/>
    /// <seealso cref="TransientInstanceProvider{TImplementation}"/>
    /// <seealso cref="CustomInstanceProvider{TImplementation}"/>
    /// <seealso cref="IInstanceProvider"/>
    /// <seealso cref="SafeInstanceProviderBase{TImplementation}"/>
    internal class SingletonInstanceProvider<TImplementation> : SafeInstanceProviderBase<TImplementation> where TImplementation : new()
    {
        private TImplementation _instance;
        public SingletonInstanceProvider(InstanceProtectionMode initialProtectionMode) : base(LifeTime.Singleton, initialProtectionMode) { }

        /// <summary>
        ///     Internal constructor with all dependencies.
        /// </summary>
        internal SingletonInstanceProvider(IInjectionDetector injectionDetector, InstanceProtectionMode protectionMode,
            InjectionAlertChannel alertChannel)
            : base(LifeTime.Singleton, injectionDetector, protectionMode, alertChannel)
        {
        }

        public override TImplementation GetInstance()
        {
            if(_instance == null) _instance = new TImplementation();
            return _instance;
        }
    }
}