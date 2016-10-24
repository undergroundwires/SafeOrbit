
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

using SafeOrbit.Memory;

namespace SafeOrbit.UnitTests
{
    internal static class SafeBytesHelper
    {
        /// <summary>
        /// Class SafeBytesHelper.
        /// </summary>
        /// <summary>
        /// Appends the and return deep clone.
        /// </summary>
        /// <param name="safeBytes">The safe bytes.</param>
        /// <param name="byte">The byte to add.</param>
        /// <returns>DeepClone of appended <see cref="@byte"/>.</returns>
        public static ISafeBytes AppendAndReturnDeepClone(this ISafeBytes safeBytes, byte @byte)
        {
            safeBytes.Append(@byte);
            return safeBytes.DeepClone();
        }
    }
}