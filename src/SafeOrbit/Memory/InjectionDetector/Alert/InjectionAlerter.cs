
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

using System;
using SafeOrbit.Library;
using SafeOrbit.Memory.Injection;
using SafeOrbit.Memory.InjectionServices.Alerters;

namespace SafeOrbit.Memory.InjectionServices
{
    /// <summary>
    /// Alerts an <see cref="IInjectionMessage"/> using the right <see cref="IAlerter"/> instance.
    /// </summary>
    /// <seealso cref="IInjectionAlerter" />
    /// <seealso cref="IAlerter"/>
    internal class InjectionAlerter : IInjectionAlerter
    {
        private readonly IAlerterFactory _alerterFactory;

        public static readonly IInjectionAlerter StaticInstance = new InjectionAlerter(new AlerterFactory());
        internal InjectionAlerter(IAlerterFactory alerterFactory)
        {
            if (alerterFactory == null) throw new ArgumentNullException(nameof(alerterFactory));
            _alerterFactory = alerterFactory;
        }

        public void Alert(IInjectionMessage info, InjectionAlertChannel channel)
        {
            var alerter = _alerterFactory.Get(channel);
            alerter.Alert(info);
        }
    }
}