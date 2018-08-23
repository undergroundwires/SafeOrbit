
/*
MIT License

Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SafeOrbit.Library.Build;
using SafeOrbit.Library.StartEarly;
using SafeOrbit.Memory;
using SafeOrbit.Memory.Injection;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Library
{
    /// <summary>
    ///     The class where you control the current library behavior.
    /// </summary>
    /// <seealso cref="Current" />
    /// <seealso cref="ISafeOrbitCore" />
    /// <seealso cref="IAlerts" />
    public class SafeOrbitCore : ISafeOrbitCore, IAlerts
    {
        public const SafeContainerProtectionMode DefaultInnerFactoryProtectionMode =
            SafeContainerProtectionMode.FullProtection;

        private static readonly Lazy<SafeOrbitCore> CurrentLazy = new Lazy<SafeOrbitCore>(() => new SafeOrbitCore());

        /// <summary>
        ///     Use static <see cref="Current" /> property instead.
        /// </summary>
        internal SafeOrbitCore()
        {
            Factory = SetupFactory();
        }

        /// <summary>
        ///     Returns the current <see cref="SafeOrbitCore" /> class for running SafeOrbit.
        /// </summary>
        public static SafeOrbitCore Current => CurrentLazy.Value;


        public InjectionAlertChannel AlertChannel
        {
            get => Factory.AlertChannel;
            set => Factory.AlertChannel = value;
        }

        public bool CanAlert => Factory.CanAlert;
        public event EventHandler<IInjectionMessage> LibraryInjected;

        public ISafeContainer Factory { get; }

        public IBuildInfo BuildInfo => new BuildInfo();


        public void StartEarly()
        {
            var factory = Factory;
            var tasks = GetAllStartEarlyTasks(factory); //get all tasks
            var actions = tasks.Select(t => new Action(t.Prepare)).ToArray(); //convert them into actions
            Parallel.Invoke(actions); //run them in parallel
        }

        /// <summary>
        ///     Internal method to alert an injection. It's virtual for testability.
        /// </summary>
        /// <param name="object">The injected object.</param>
        /// <param name="info">The information.</param>
        internal virtual void AlertInjection(object @object, IInjectionMessage info)
        {
            var localCopy = LibraryInjected;
            localCopy?.Invoke(@object, info);
        }

        private static ISafeContainer SetupFactory()
        {
            var result = new SafeContainer(DefaultInnerFactoryProtectionMode);
            FactoryBootstrapper.Bootstrap(result);
            result.Verify();
            return result;
        }

        private static IEnumerable<IStartEarlyTask> GetAllStartEarlyTasks(ISafeContainer container)
        {
            yield return new SafeByteFactoryInitializer(container);
            yield return new StartFillingEntropyPoolsStartEarlyTask();
        }
    }
}