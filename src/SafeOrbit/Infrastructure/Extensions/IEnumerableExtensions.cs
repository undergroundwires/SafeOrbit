﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SafeOrbit.Extensions
{
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

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> iEnumerable)
            => iEnumerable == null || !iEnumerable.Any();
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

        /// <summary>
        ///     Compares and returns if all elements equal to each other.
        /// </summary>
        public static bool AreAllEqual<T>(this IEnumerable<T> sequence)
        {
            return !sequence.Any() || sequence.Distinct().Count() == 1;
        }
    }
}