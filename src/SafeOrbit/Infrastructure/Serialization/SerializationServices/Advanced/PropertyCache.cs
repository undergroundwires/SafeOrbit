
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

//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System;
using System.Collections.Generic;
using System.Reflection;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced
{
    ///<summary>
    ///</summary>
    internal class PropertyCache
    {
        private readonly Dictionary<Type, IList<PropertyInfo>> _cache = new Dictionary<Type, IList<PropertyInfo>>();

        /// <summary>
        /// </summary>
        /// <returns>null if the key was not found</returns>
        public IList<PropertyInfo> TryGetPropertyInfos(Type type)
        {
            if (!_cache.ContainsKey(type))
            {
                return null;
            }
            return _cache[type];
        }

        public void Add(Type key, IList<PropertyInfo> value)
        {
            _cache.Add(key, value);
        }
    }
}