using System;
using System.Threading;
using System.Threading.Tasks;

namespace SafeOrbit.Extensions
{
    internal static class TaskExtensions
    {
        private static readonly TaskFactory MyTaskFactory = new
            TaskFactory(CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

        public static TResult RunSync<TResult>(this Func<Task<TResult>> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            return MyTaskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }

        public static void RunSync(this Func<Task> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            MyTaskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }

        public static void RunSync(this Task task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            task
                .GetAwaiter()
                .GetResult();
        }

        public static TResult RunSync<TResult>(this Task<TResult> task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            return
                task
                    .GetAwaiter()
                    .GetResult();
        }
    }
}