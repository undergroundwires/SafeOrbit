
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

namespace SafeOrbit.Memory
{
    /// <summary>
    /// Specifies the lifetime of a service in an <see cref="IInstanceProvider"/>.
    /// </summary>
    /// <seealso cref="IInstanceProvider"/>
    public enum LifeTime
    {
        /// <summary>
        /// <see cref="Singleton"/> life time is a static, single instance of the class.
        /// <see cref="SafeContainer"/> returns the same instance for each request.
        /// Instances that are declared as <see cref="Singleton"/> should be thread-safe in a multi-threaded environment.
        /// </summary>
        Singleton,
        /// <summary>
        /// Transient are created each time they are requested.
        /// <see cref="SafeContainer"/> returns a new instance of the for after each request.
        /// </summary>
        Transient,
        /// <summary>
        /// Represents lifetime that's unknown to the SafeContainer.
        /// </summary>
        Unknown
    }
}