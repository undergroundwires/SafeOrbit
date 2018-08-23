/*
MIT License

Copyright (c) 2016-2018 Erkin Ekici - undergroundwires@safeorb.it

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
using SafeOrbit.Library.Build;
using SafeOrbit.Memory;
using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Library
{
    /// <summary>
    ///     Abstracts the class to access inner library behavior.
    /// </summary>
    public interface ISafeOrbitCore
    {
        /// <summary>
        ///     Safe object container that's being used by the library.
        /// </summary>
        ISafeContainer Factory { get; }

        /// <summary>
        ///     Loads the necessary data early on. For better performance, it's highly recommended to start the
        ///     application early in your application start.
        /// </summary>
        void StartEarly();

        event EventHandler<IInjectionMessage> LibraryInjected;


        /// <summary>
        /// Gets the information regarding to current build of SafeOrbit.
        /// </summary>
        IBuildInfo BuildInfo { get; }
    }
}