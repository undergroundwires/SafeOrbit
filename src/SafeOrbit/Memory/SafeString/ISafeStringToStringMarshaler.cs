
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

namespace SafeOrbit.Memory
{
    /// <summary>
    ///     Abstract a marshaler that can convert a <see cref="ISafeString" /> into a <see cref="string" /> until it is
    ///     disposed
    /// </summary>
    /// <seealso cref="IDisposable" />
    /// <seealso cref="ISafeString" />
    /// <seealso cref="SafeString" />
    /// <seealso cref="string" />
    public interface ISafeStringToStringMarshaler : IDisposable
    {
        /// <summary>
        ///     Gets or sets the safe string.
        /// </summary>
        /// <value>The safe string.</value>
        ISafeString SafeString { get; set; }

        /// <summary>
        ///     Gets the disposable string.
        /// </summary>
        /// <value>The disposable string.</value>
        string String { get; }
    }
}