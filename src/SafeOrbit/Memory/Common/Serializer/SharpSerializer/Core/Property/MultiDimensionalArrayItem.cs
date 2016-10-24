
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

namespace SafeOrbit.Memory.Serialization.SerializationServices.Core
{
    /// <summary>
    ///     Information about one item in a multidimensional array
    /// </summary>
    internal sealed class MultiDimensionalArrayItem
    {
        /// <summary>
        /// </summary>
        /// <param name="indexes"></param>
        /// <param name="value"></param>
        public MultiDimensionalArrayItem(int[] indexes, Property value)
        {
            Indexes = indexes;
            Value = value;
        }

        /// <summary>
        ///     Represents item coordinates in the array (i.e. [1,5,3] - item has index 1 in the dimension 0, index 5 in the
        ///     dimension 1 and index 3 in the dimension 2).
        /// </summary>
        public int[] Indexes { get; set; }

        /// <summary>
        ///     Item value. It can contain any type.
        /// </summary>
        public Property Value { get; set; }
    }
}