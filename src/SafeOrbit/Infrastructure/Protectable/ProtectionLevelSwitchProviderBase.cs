
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
using System.Threading;

namespace SafeOrbit.Infrastructure.Protectable
{
    /// <summary>
    ///     A base class that provides helper methods for the implementation of
    ///     <see cref="IProtectable{TProtectionLevel}" />.
    /// </summary>
    /// <typeparam name="TProtectionLevel">The type of the t protection level.</typeparam>
    /// <seealso cref="IProtectable{TProtectionLevel}" />
    public abstract class ProtectableBase<TProtectionLevel> :
        IProtectable<TProtectionLevel> where TProtectionLevel : struct
    {
        /// <summary>
        ///     A flag that indicates mode is being set in any thread.
        /// </summary>
        private volatile bool _isSettingMode;


        protected ProtectableBase(TProtectionLevel protectionMode)
        {
            CurrentProtectionMode = protectionMode;
        }

        /// <summary>
        ///     Gets the current protection mode.
        /// </summary>
        /// <value>The current protection mode.</value>
        public TProtectionLevel CurrentProtectionMode { get; private set; }

        /// <summary>
        ///     Sets the <see cref="CurrentProtectionMode" /> if the value of <paramref name="protectionMode" /> is different than
        ///     the <see cref="CurrentProtectionMode" />.
        /// </summary>
        /// <param name="protectionMode">The object protection mode.</param>
        public void SetProtectionMode(TProtectionLevel protectionMode)
        {
            if (protectionMode.Equals(CurrentProtectionMode)) return;
            InternalSetMode(protectionMode);
        }

        protected void SpinUntilSecurityModeIsAvailable()
        {
            if (_isSettingMode)
            {
                SpinWait.SpinUntil(() => _isSettingMode, 10000);
            }
        } 

        /// <summary>
        ///     Must be overridden with a logic while switching happens.
        /// </summary>
        protected abstract void ChangeProtectionMode(IProtectionChangeContext<TProtectionLevel> context);

        /// <summary>
        ///     Calls the <see cref="ChangeProtectionMode" /> method with right context. If the operation is not canceled then
        ///     sets <see cref="CurrentProtectionMode" />
        /// </summary>
        /// <param name="objectProtectionMode">The object protection mode.</param>
        private void InternalSetMode(TProtectionLevel objectProtectionMode)
        {
            if (_isSettingMode) throw new Exception("Another thread is currently setting the protection level.");
            _isSettingMode = true;
            var context = GetContext(newValue: objectProtectionMode, oldValue: CurrentProtectionMode);
            ChangeProtectionMode(context);
            if (!context.IsCanceled)
                CurrentProtectionMode = objectProtectionMode;
            _isSettingMode = false;
        }

        private IProtectionChangeContext<TProtectionLevel> GetContext(TProtectionLevel oldValue,
            TProtectionLevel newValue)
        {
            var result = new ProtectionChangeContext<TProtectionLevel>
            (
                newValue: newValue,
                oldValue: oldValue
            );
            return result;
        }
    }
}