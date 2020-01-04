using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SafeOrbit.Helpers
{
    public class Fast
    {
        /// <summary>
        ///     Runs fast for-each using <see cref="Parallel" /> and <see cref="Partitioner" />
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="delegate">The delegate.</param>
        /// <exception cref="ArgumentNullException"><paramref name="delegate" /> is <see langword="null" />.</exception>
        public static void For(int startIndex, int endIndex, Action<int> @delegate)
        {
            if (@delegate == null) throw new ArgumentNullException(nameof(@delegate));
            for (var i = startIndex; i < endIndex; i++)
            {
                @delegate.Invoke(i);
            }
//#if !NETSTANDARD1_6
//            var partitioner = Partitioner.Create(0, endIndex);
//            Parallel.ForEach(partitioner, range =>
//            {
//                for (var i = range.Item1; i < range.Item2; i++)
//                    @delegate.Invoke(i);
//            });
//#elsesafeenc
//            for (var i = startIndex; i < endIndex; i++)
//            {
//                @delegate.Invoke(i);
//            }
//#endif
        }
    }
}