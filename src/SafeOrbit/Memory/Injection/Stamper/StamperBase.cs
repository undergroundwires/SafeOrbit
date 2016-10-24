
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
using SafeOrbit.Hash;
using SafeOrbit.Memory.Injection;

namespace SafeOrbit.Memory.InjectionServices.Stampers
{
    internal abstract class StamperBase<TObject>: IStamper<TObject>
    {
        protected abstract byte[] GetSerializedBytes(TObject @object);
        public abstract InjectionType InjectionType { get; }
        private readonly IFastHasher _fastHasher;
        protected StamperBase(IFastHasher fastHasher)
        {
            _fastHasher = fastHasher;
        }
        protected int Hash(byte[] input) => _fastHasher.ComputeFast(input);
        private int SerializeAndHash(TObject @object)
        {
            var serialization = GetSerializedBytes(@object);
            var hash = _fastHasher.ComputeFast(serialization);
            return hash;
        }
        public IStamp<int> GetStamp(TObject @object)
        {
            var hash = SerializeAndHash(@object);
            return new Stamp(hash);
        }
    }
}