using System;
using System.Threading.Tasks;

namespace SafeOrbit.Parallel
{
    public class AsyncLazy<T>
    {
        private readonly Lazy<Task<T>> _lazy;
        public AsyncLazy(Func<Task<T>> initialize)
        {
            if (initialize == null)
            {
                throw new ArgumentNullException(nameof(initialize));
            }
            _lazy = new Lazy<Task<T>>(initialize);
        }
        public Task<T> ValueAsync() => _lazy.Value;
    }
}
