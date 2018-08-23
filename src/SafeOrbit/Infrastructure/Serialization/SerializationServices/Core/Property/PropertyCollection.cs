
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

using System.Collections.ObjectModel;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Core
{
    /// <summary>
    ///     It represents some properties of an object, or some items of a collection/dictionary/array
    /// </summary>
    internal sealed class PropertyCollection : Collection<Property>
    {
        /// <summary>
        ///     Parent property
        /// </summary>
        public Property Parent { get; set; }

        /// <summary>
        /// </summary>
        protected override void ClearItems()
        {
            foreach (var item in Items)
                item.Parent = null;
            base.ClearItems();
        }

        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, Property item)
        {
            base.InsertItem(index, item);
            item.Parent = Parent;
        }

        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            Items[index].Parent = null;
            base.RemoveItem(index);
        }

        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, Property item)
        {
            Items[index].Parent = null;
            base.SetItem(index, item);
            item.Parent = Parent;
        }
    }
}