
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

//This is a modified version of the beautiful SharpSerializer by Pawel Idzikowski (see: http://www.sharpserializer.com)

using System.Collections.Generic;

namespace SafeOrbit.Memory.Serialization.SerializationServices.Core.Binary
{
    /// <summary>
    ///     Is used to store types and property names in lists. Contains only unique elements and gives index of the item back.
    ///     During deserialization this index is read from stream and then replaced with an appropriate value from the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class IndexGenerator<T>
    {
        private readonly List<T> _items = new List<T>();


        public IList<T> Items => _items;

        /// <summary>
        ///     if the item exist, it gives its index back, otherwise the item is added and its new index is given back
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetIndexOfItem(T item)
        {
            var index = _items.IndexOf(item);

            // item was found
            if (index > -1) return index;

            // item was not found
            _items.Add(item);
            return _items.Count - 1;
        }
    }
}