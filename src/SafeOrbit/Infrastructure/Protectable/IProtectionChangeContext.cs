
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

namespace SafeOrbit.Infrastructure.Protectable
{
    /// <summary>
    ///     An interface representing the event arguments when the protection level switch is requested.
    /// </summary>
    /// <typeparam name="TProtectionLevel">The type of the protection level.</typeparam>
    public interface IProtectionChangeContext<out TProtectionLevel>
    {
        /// <summary>
        ///     Gets the old value of the <typeparamref name="TProtectionLevel" />.
        /// </summary>
        /// <value>The old value of the <typeparamref name="TProtectionLevel" />.</value>
        TProtectionLevel OldValue { get; }

        /// <summary>
        ///     Gets the new value of the <typeparamref name="TProtectionLevel" />. This is the value that's requested to be set.
        /// </summary>
        /// <value>The new value of the <typeparamref name="TProtectionLevel" /> that's requested to be set.</value>
        TProtectionLevel NewValue { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the protection level switching is canceled.
        /// </summary>
        /// <value><c>true</c> if this protection level switching is canceled; otherwise, <c>false</c>.</value>
        bool IsCanceled { get; set; }
    }
}