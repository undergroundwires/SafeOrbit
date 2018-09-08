using System;
using System.Runtime.CompilerServices;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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