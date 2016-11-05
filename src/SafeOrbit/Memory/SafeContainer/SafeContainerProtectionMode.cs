
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

namespace SafeOrbit.Memory
{
    /// <summary>
    ///     Protection levels for <see cref="ISafeContainer" /> instances.
    /// </summary>
    /// <seealso cref="ISafeContainer" />
    public enum SafeContainerProtectionMode
    {
        /// <summary>
        ///     Protection for both state and code of its instances.
        ///     It's slowest and the safest mode.
        ///     <seealso cref="InjectionType.CodeAndVariableInjection" />
        ///     <seealso cref="IInjectionDetector" />
        /// </summary>
        FullProtection,

        /// <summary>
        ///     No protection for either the code or the state of the objects.
        ///     It's much faster than <see cref="FullProtection" /> but provides no security against injections.
        /// </summary>
        NonProtection
    }
}