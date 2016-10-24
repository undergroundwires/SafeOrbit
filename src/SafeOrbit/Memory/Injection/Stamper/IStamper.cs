
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

using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Memory.InjectionServices.Stampers
{
    /// <summary>
    ///     Abstraction for the service that stamps objects.
    /// </summary>
    /// <typeparam name="TObject">Type of object.</typeparam>
    /// <typeparam name="TStamp">The type of the the stamp.</typeparam>
    internal interface IStamper<in TObject, out TStamp>
    {
        InjectionType InjectionType { get; }
        TStamp GetStamp(TObject @object);
    }

    /// <summary>
    ///     Abstraction for the service that stamps objects with an <see cref="IStamp{THash}" /> stamp.
    /// </summary>
    /// <typeparam name="TObject">The type of the t object.</typeparam>
    internal interface IStamper<in TObject> : IStamper<TObject, IStamp<int>>
    {
    }
}