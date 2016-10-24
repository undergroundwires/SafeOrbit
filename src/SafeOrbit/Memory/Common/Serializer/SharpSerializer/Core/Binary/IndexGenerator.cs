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