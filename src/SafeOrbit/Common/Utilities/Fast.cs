using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SafeOrbit.Utilities
{
    public class Fast
    {
        /// <summary>
        ///     Runs fast for-each using <see cref="Parallel" /> and <see cref="Partitioner" />
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="delegate">The delegate.</param>
        /// <exception cref="ArgumentNullException"><paramref name="@delegate" /> is <see langword="null" />.</exception>
        public static void For(int startIndex, int endIndex, Action<int> @delegate)
        {
            if (@delegate == null) throw new ArgumentNullException(nameof(@delegate));
            var partitioner = Partitioner.Create(0, endIndex);
            var task = Parallel.ForEach(partitioner, range =>
            {
                for (var i = range.Item1; i < range.Item2; i++)
                    @delegate.Invoke(i);
            });
        }
    }
}