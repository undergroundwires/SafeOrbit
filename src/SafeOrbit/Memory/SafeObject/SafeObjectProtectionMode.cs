
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

namespace SafeOrbit.Memory
{
    /// <summary>
    ///     Different protection modes for a <see cref="ISafeObject{TObject}" />
    /// </summary>
    /// <seealso cref="ISafeObject{TObject}" />
    public enum SafeObjectProtectionMode
    {
        /// <summary>
        ///     Provides the maximum security by protecting the object against both state and code injections
        /// </summary>
        StateAndCode = 3,

        /// <summary>
        ///     Provides protection against only state injections. This option should be selected if the code of the object is
        ///     designed to change its  code dynamically. The type will be vulnerable to code injections.
        /// </summary>
        JustState = 2,

        /// <summary>
        ///     Provides protection against only code injections. This option might be good if the object is a stateless class, or
        ///     the data it's holding is not sensitive.
        /// </summary>
        JustCode = 1,

        /// <summary>
        ///     Provides no protection / security.
        ///     <b>Important : This option is completely un-safe and will disable injection scans.</b>
        /// </summary>
        NoProtection = 0
    }
}