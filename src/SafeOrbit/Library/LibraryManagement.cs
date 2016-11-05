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
using SafeOrbit.Library.StartEarly;
using SafeOrbit.Memory;
using SafeOrbit.Memory.Injection;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Library
{
    public class LibraryManagement
    {
        public const SafeContainerProtectionMode DefaultInnerFactoryProtectionMode =
            SafeContainerProtectionMode.FullProtection;

        /// <summary>
        ///     Returns the current <see cref="LibraryManagement" /> class for running SafeOrbit.
        /// </summary>
        public static LibraryManagement Current = new LibraryManagement();

        public static EventHandler<IInjectionMessage> LibraryInjected;


        /// <summary>
        ///     Static holder for <see cref="Factory" />
        /// </summary>
        private static readonly Lazy<ISafeContainer> FactoryLazy = new Lazy<ISafeContainer>(SetupFactory);

        /// <summary>
        ///     Use static <see cref="Current" /> property instead.
        /// </summary>
        internal LibraryManagement()
        {
        }

        public ISafeContainer Factory => FactoryLazy.Value;

        public SafeContainerProtectionMode ProtectionMode
        {
            get { return Factory.CurrentProtectionMode; }
            set
            {
                if (value != Factory.CurrentProtectionMode) Factory.SetProtectionMode(value);
            }
        }

        public InjectionAlertChannel AlertChannel
        {
            get { return Factory.AlertChannel; }
            set
            {
               Factory.AlertChannel = value;
            }
        }

        public void StartEarly(
            SafeContainerProtectionMode protectionMode = SafeContainerProtectionMode.NonProtection)
        {
            var factory = this.Factory;
            var tasks = GetAllStartEarlyTasks(factory); //get all tasks
            var actions = tasks.Select(t => new Action(t.Prepare)).ToArray(); //convert them into actions
            Parallel.Invoke(actions); //run them in parallel
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