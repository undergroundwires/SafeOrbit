using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SafeOrbit.Utilities
{
    public static class New<T>
    {
        /// <summary>
        /// Creates a fast new instance.
        /// ~50 ms for classes and ~100 ms for structs.
        /// approx. 20 times faster than reflection.
        /// </summary>
        public static readonly Func<T> Instance = Expression.Lambda<Func<T>>
                                                  (
                                                   Expression.New(typeof(T))
                                                  ).Compile();     

        public static bool HasDefaultConstructor => typeof(T).GetConstructor(Type.EmptyTypes) != null;
    }
}
