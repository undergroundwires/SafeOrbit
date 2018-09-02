using System;

#if !NETCORE
using System.Runtime.CompilerServices;
#endif

namespace SafeOrbit.Helpers
{
    public class RuntimeHelper
    {
        public static void ExecuteCodeWithGuaranteedCleanup(Action action, Action cleanup)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (cleanup == null) throw new ArgumentNullException(nameof(cleanup));

#if !NETCORE
            RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(
            delegate
                {
                    action.Invoke();
                }
                ,
            delegate
            {
                cleanup.Invoke();
            },
                null);
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
        /// Create a CER (Constrained Execution Region)
        /// </summary>
        public static void PrepareConstrainedRegions()
        {
#if !NETCORE
            RuntimeHelpers.PrepareConstrainedRegions();
#endif
        }
    }
}