using System;
using System.Threading.Tasks;

namespace SafeOrbit.Extensions
{
    public static class TaskContext
    {
        public static T RunSync<T>(this Func<Task<T>> func)
            => func == null ? throw new ArgumentNullException(nameof(func)) : 
               func.RunOnDefaultScheduler().RunSync();
        public static void RunSync(this Func<Task> func) => func.RunOnDefaultScheduler().RunSync();
        // task.GetAwaiter().GetResult() :
        //      Run sync & fixes stack traces on exceptions https://referencesource.microsoft.com/#mscorlib/system/runtime/compilerservices/TaskAwaiter.cs,ca9850c71672bd54
        private static void RunSync(this Task task) => task.GetAwaiter().GetResult();
        private static T RunSync<T>(this Task<T> task) => task == null ? throw new ArgumentNullException(nameof(task)) : 
            task.GetAwaiter().GetResult();
    }
}