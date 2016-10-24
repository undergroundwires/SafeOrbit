
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

namespace SafeOrbit.Memory.Common
{
    /// <summary>
    /// A basic class that implements <see cref="IProtectionLevelSwitchingContext{TProtectionLevel}"/>. This class cannot be inherited.
    /// </summary>
    /// <typeparam name="TProtectionLevel">The type of the protection level.</typeparam>
    /// <seealso cref="IProtectionLevelSwitchingContext{TProtectionLevel}" />
    internal sealed class ProtectionLevelSwitchingContext<TProtectionLevel> : IProtectionLevelSwitchingContext<TProtectionLevel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectionLevelSwitchingContext{TProtectionLevel}"/> class using an old an new value.
        /// </summary>
        /// <param name="oldValue">The old value of the protection level.</param>
        /// <param name="newValue">The new value of the protection level.</param>
        public ProtectionLevelSwitchingContext(TProtectionLevel oldValue, TProtectionLevel newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectionLevelSwitchingContext{TProtectionLevel}"/> class using the new value.
        /// </summary>
        /// <param name="newValue">The new value of the protection level.</param>
        public ProtectionLevelSwitchingContext(TProtectionLevel newValue)
        {
            NewValue = newValue;
        }

        /// <summary>
        /// Gets the old value of the <see cref="TProtectionLevel"/>.
        /// </summary>
        /// <value>The old value of the <see cref="TProtectionLevel"/>.</value>
        public TProtectionLevel OldValue { get; }
        /// <summary>
        /// Gets the new value of the <see cref="TProtectionLevel"/>. This is the value that's requested to be set.
        /// </summary>
        /// <value>The new value of the <see cref="TProtectionLevel"/> that's requested to be set.</value>
        public TProtectionLevel NewValue { get; }
        /// <summary>
        /// Gets or sets a value indicating whether the protection level switching is canceled.
        /// </summary>
        /// <value><c>true</c> if this protection level switching is canceled; otherwise, <c>false</c>.</value>
        public bool IsCanceled { get; set; }
    }
}