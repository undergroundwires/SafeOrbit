
/*
MIT License

Copyright (c) 2018 Erkin Ekici - undergroundwires@safeorb.it

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

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
        ///     Invokes the static constructor for <typeparamref name="TStatic" />.
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