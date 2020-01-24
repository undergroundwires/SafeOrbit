using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SafeOrbit.Parallel
{
    public static class TaskContext
    {
        /// <summary>
        ///     Executes an async Task&lt;T&gt; method which has a T return type synchronously
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="task">Task&lt;T&gt; method to execute</param>
        /// <returns>Returns the waited result of the task</returns>
        [DebuggerHidden]
        public static T RunSync<T>(this Func<Task<T>> task)
            => task == null ? throw new ArgumentNullException(nameof(task)) : task.RunOnDefaultScheduler().RunSync();

        /// <summary>
        ///     Executes an async Task method which has a void return value synchronously
        /// </summary>
        /// <param name="task">Task method to execute</param>
        [DebuggerHidden]
        public static void RunSync(this Func<Task> task)
            => task.RunOnDefaultScheduler().RunSync();

        [DebuggerHidden]
        private static void RunSync(this Task task) =>
            // task.GetAwaiter().GetResult() :
            // Run sync & fixes stack traces on exceptions https://referencesource.microsoft.com/#mscorlib/system/runtime/compilerservices/TaskAwaiter.cs,ca9850c71672bd54
            task.ConfigureAwait(false).GetAwaiter().GetResult();

        [DebuggerHidden]
        private static T RunSync<T>(this Task<T> task) => task == null
            ? throw new ArgumentNullException(nameof(task))
            : task.ConfigureAwait(false).GetAwaiter().GetResult();
    }
}