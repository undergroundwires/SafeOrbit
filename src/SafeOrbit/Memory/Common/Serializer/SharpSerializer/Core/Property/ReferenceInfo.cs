
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
    ///     Provides information about property references
    /// </summary>
    internal sealed class ReferenceInfo
    {
        ///<summary>
        ///</summary>
        public ReferenceInfo()
        {
            Count = 1;
        }

        /// <summary>
        ///     How many references to the same object
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///     Every Object must have a unique Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     During serialization is true if the target object was already serialized.
        ///     Then the target must not be serialized again. Only its reference must be created.
        ///     During deserialization it means, the target object was parsed and read
        ///     from the stream. It can be further used to resolve its references.
        /// </summary>
        public bool IsProcessed { get; set; }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public override string ToString()
        {
            return $"{GetType().Name}, Count={Count}, Id={Id}, IsProcessed={IsProcessed}";
        }
    }
}