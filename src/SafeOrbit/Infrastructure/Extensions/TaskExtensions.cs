using System;
using System.Threading;
using System.Threading.Tasks;

namespace SafeOrbit.Extensions
{
    /// <summary>
    /// Stephen Cleary approved
    /// http://stackoverflow.com/questions/15428604/how-to-run-a-task-on-a-custom-taskscheduler-using-await
    /// </summary>
    internal static class TaskExtensions
    {
        private static readonly TaskFactory TaskFactory = new
            TaskFactory(CancellationToken.None,
                TaskCreationOptions.DenyChildAttach,
                TaskContinuationOptions.None,
                TaskScheduler.Default);
        internal static Task RunOnDefaultScheduler(this Func<Task> func)
        {
            return TaskFactory.StartNew(func).Unwrap();
        }

        internal static Task<T> RunOnDefaultScheduler<T>(this Func<Task<T>> func)
        {
            return TaskFactory.StartNew(func).Unwrap();
        }
    }
}