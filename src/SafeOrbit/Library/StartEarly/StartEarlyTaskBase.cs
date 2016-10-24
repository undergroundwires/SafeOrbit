using System.Runtime.CompilerServices;

namespace SafeOrbit.Library.StartEarly
{
    /// <summary>
    /// Base class for <see cref="IStartEarlyTask"/>'s with some helper methods.
    /// </summary>
    /// <seealso cref="IStartEarlyTask" />
    internal abstract class StartEarlyTaskBase : IStartEarlyTask
    {
        public abstract void Prepare();

        /// <summary>
        ///     Invokes the static constructor for
        ///     <typeparam name="TStatic" />
        ///     .
        ///     Guarantees that the static constructor is only called once, regardless how many times the method is called.
        /// </summary>
        /// <remarks>
        ///     <p>
        ///         More at:
        ///         https://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.runtimehelpers.runclassconstructor%28v=vs.110%29.aspx
        ///     </p>
        /// </remarks>
        /// <typeparam name="TStatic">The class with a static constructor</typeparam>
        protected void InvokeStaticConstructorFor<TStatic>()
        {
            var type = typeof(TStatic);
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }
    }
}