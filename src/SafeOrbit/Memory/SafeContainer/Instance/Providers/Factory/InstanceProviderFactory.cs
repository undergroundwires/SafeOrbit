
/*
MIT License

Copyright (c) 2018 Erkin Ekici - undergroundwires@safeorb.it

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
using System.Collections.Generic;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Memory.SafeContainerServices.Instance.Providers
{
    /// <summary>
    /// Returns the right
    /// </summary>
    /// <seealso cref="IInstanceProviderFactory" />
    internal class InstanceProviderFactory : IInstanceProviderFactory
    {
        public static IInstanceProviderFactory StaticInstance => StaticInstanceLazy.Value;
        public static Lazy<InstanceProviderFactory> StaticInstanceLazy = new Lazy<InstanceProviderFactory>();
        public IInstanceProvider Get<TImplementation>(LifeTime lifeTime, InstanceProtectionMode protectionMode,
            InjectionAlertChannel alertChannel) where TImplementation : new()
        {
            IInstanceProvider instance;
            switch (lifeTime)
            {
                case LifeTime.Singleton:
                    instance = new SingletonInstanceProvider<TImplementation>(protectionMode);
                    break;
                case LifeTime.Transient:
                    instance = new TransientInstanceProvider<TImplementation>(protectionMode);
                    break;
                case LifeTime.Unknown:
                    throw new ArgumentOutOfRangeException($"You can only register {LifeTime.Unknown} instances with a {nameof(Func<IInstanceProvider>)} parameter.Please check overloaded method.s");
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifeTime), lifeTime, null);
            }
            instance.AlertChannel = alertChannel;
            return instance;
        }

        public IInstanceProvider Get<TImplementation>(Func<TImplementation> instanceGetter, InstanceProtectionMode protectionMode,
            InjectionAlertChannel alertChannel, LifeTime lifeTime = LifeTime.Unknown)
        {
            var result = new CustomInstanceProvider<TImplementation>(
                lifeTime: lifeTime,
                instanceGetter: instanceGetter,
                protectionMode: protectionMode)
            {
                AlertChannel = alertChannel
            };
            result.SetProtectionMode(protectionMode);
            return result;
        }
    }
}