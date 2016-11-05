using System;
#if NET46
using System.Runtime.CompilerServices;

#endif

namespace SafeOrbit.Utilities
{
    public class RuntimeHelper
    {
        public static void ExecuteCodeWithGuaranteedCleanup(Action action, Action cleanup)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (cleanup == null) throw new ArgumentNullException(nameof(cleanup));

#if NET46
            RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(action
                , cleanup,
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
    }
}