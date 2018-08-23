
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
using System.Linq;
using System.Reflection;
using SafeOrbit.Infrastructure.Reflection;
using SafeOrbit.Cryptography.Hashers;

namespace SafeOrbit.Memory.InjectionServices.Stampers
{
    /// <summary>
    ///     Stamps <see cref="Type" /> of an object by serializing its IL-code.
    /// </summary>
    /// <seealso cref="StamperBase{Type}" />
    /// <seealso cref="IStamper{Type}" />
    internal class IlCodeStamper : StamperBase<Type>
    {
        public static IlCodeStamper StaticInstance = new IlCodeStamper();

        public IlCodeStamper() : this(Murmur32.StaticInstance)
        {
        }

        internal IlCodeStamper(IFastHasher fastHasher) : base(fastHasher)
        {
        }

        public override InjectionType InjectionType { get; } = InjectionType.CodeInjection;

        protected override byte[] GetSerializedBytes(Type @object)
        {
            var methods = @object.GetMethods();
            var bytes = methods.Select(m => m.GetIlBytes()) //get all
                .Where(b=> b != null) //remove nulls
                .SelectMany(b=> b) //merge
                .ToArray();
            if ((bytes == null) || !bytes.Any()) return new byte[16]; //return bulk bytes
            return bytes;
        }
    }
}