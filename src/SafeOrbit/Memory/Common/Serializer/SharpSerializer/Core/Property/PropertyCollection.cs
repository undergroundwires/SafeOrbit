using System.Collections.ObjectModel;

namespace SafeOrbit.Memory.Serialization.SerializationServices.Core
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