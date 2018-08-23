
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

using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Infrastructure.Protectable
{
    /// <summary>
    ///     Defines a class that can work in different protection modes, and can dynamically switch between them.
    /// </summary>
    /// <typeparam name="TProtectionLevel">The type of the protection model.</typeparam>
    public interface IProtectable<TProtectionLevel>
        where TProtectionLevel : struct
    {
        /// <summary>
        ///     Gets the current protection mode.
        /// </summary>
        /// <value>The current protection mode.</value>
        TProtectionLevel CurrentProtectionMode { get; }

        /// <summary>
        ///     Sets the protection mode.
        /// </summary>
        /// <param name="objectProtectionMode">The object protection mode.</param>
        void SetProtectionMode(TProtectionLevel objectProtectionMode);
    }
}