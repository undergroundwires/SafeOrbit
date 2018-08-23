
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

using SafeOrbit.Exceptions;
using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Memory.InjectionServices.Alerters
{
    /// <summary>
    ///     Throws <see cref="MemoryInjectionException" />.
    /// </summary>
    internal class ThrowExceptionAlerter : IAlerter
    {
        public InjectionAlertChannel Channel { get; } = InjectionAlertChannel.ThrowException;

        /// <exception cref="MemoryInjectionException">
        ///     <p>
        ///         The state of the object has been changed after last validation.
        ///         <seealso cref="InjectionType.VariableInjection" />
        ///     </p>
        ///     <p>
        ///         The code of the object has been changed after last validation.
        ///         <seealso cref="InjectionType.CodeInjection" />
        ///     </p>
        ///     <p>
        ///         Both state and the code code of the object has been changed after last validation.
        ///         <seealso cref="InjectionType.CodeAndVariableInjection" />
        ///     </p>
        /// </exception>
        public void Alert(IInjectionMessage info)
        {
            throw new MemoryInjectionException(info.InjectionType, info.InjectedObject, info.InjectionDetectionTime);
        }
    }
}