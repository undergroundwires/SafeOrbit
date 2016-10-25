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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SafeOrbit.Extensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class IEnumerableExtensions
    {
        /// <summary>
        ///     Performs the specified action on each element of the <see cref="IEnumerable{T}" />.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="IEnumerable{T}" /></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="action">
        ///     The <see cref="Action{T}" /> delegate to perform on each element of the
        ///     <see cref="IEnumerable{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <p><paramref name="items" /> is <see langword="null" /></p>
        /// </exception>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null) return;
            if (action == null) throw new ArgumentNullException(nameof(action));
            foreach (var item in items)
                action(item);
        }

        /// <summary>
        ///     Returns an empty <see cref="IEnumerable{T}" /> if the caller is <see langword="null" />
        /// </summary>
        /// <typeparam name="T">Type of the enumeration</typeparam>
        /// <param name="iEnumerable">The enumeration.</param>
        /// <returns>Null if <paramref name="iEnumerable" /> is null, otherwise; <paramref name="iEnumerable" /></returns>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> iEnumerable)
        {
            return iEnumerable ?? Enumerable.Empty<T>();
        }
    }
}