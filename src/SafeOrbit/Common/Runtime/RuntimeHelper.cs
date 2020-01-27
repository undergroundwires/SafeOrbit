using System;
#if !NETSTANDARD1_6
using System.Runtime.CompilerServices;

#endif

namespace SafeOrbit.Helpers
{
    public class RuntimeHelper
    {
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="action" /> or <paramref name="cleanup" /> is
        ///     <see langword="null" />
        /// </exception>
        public static void ExecuteCodeWithGuaranteedCleanup(Action action, Action cleanup)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (cleanup == null) throw new ArgumentNullException(nameof(cleanup));

#if !NETSTANDARD1_6
            //RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(
            //delegate
            //    {
            //        action.Invoke();
            //    }
            //    ,
            //delegate
            //{
            //    cleanup.Invoke();
            //},
            //    null);
            try
            {
                action.Invoke();
            }
            finally
            {
                cleanup.Invoke();
            }
#else
            try
            {
                action.Invoke();
            }
            finally
            {
                cleanup.Invoke();
            }
#endif
        }

        /// <summary>
        ///     Create a CER (Constrained Execution Region)
        /// </summary>
        public static void PrepareConstrainedRegions()
        {
#if !NETSTANDARD1_6
            RuntimeHelpers.PrepareConstrainedRegions();
#endif
        }
    }
}