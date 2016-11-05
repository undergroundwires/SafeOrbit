
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

using System;
using System.Collections.Generic;
using System.Reflection;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices.Core
{
        /// <summary>
        ///   Gives information about actually analyzed array (from the constructor)
        /// </summary>
        internal class ArrayAnalyzer
        {
            private readonly object _array;
            private readonly ArrayInfo _arrayInfo;
            private IList<int[]> _indexes;

            ///<summary>
            ///</summary>
            ///<param name = "array"></param>
            public ArrayAnalyzer(object array)
            {
                _array = array;
                var type = array.GetType();
                _arrayInfo = GetArrayInfo(type);
            }

            /// <summary>
            ///   Contains extended information about the current array
            /// </summary>
            public ArrayInfo ArrayInfo => _arrayInfo;

            /// <summary>
            ///   How many dimensions. There can be at least 1
            /// </summary>
            /// <returns></returns>
            private int GetRank(Type arrayType)
            {
                return arrayType.GetArrayRank();
            }

            /// <summary>
            ///   How many items in one dimension
            /// </summary>
            /// <param name = "dimension">0-based</param>
            /// <returns></returns>
            /// <param name="arrayType"></param>
            private int GetLength(int dimension, Type arrayType)
            {
                var methodInfo = arrayType.GetMethod("GetLength");
                var length = (int)methodInfo.Invoke(_array, new object[] { dimension });
                return length;
            }

            /// <summary>
            ///   Lower index of an array. Default is 0.
            /// </summary>
            /// <param name = "dimension">0-based</param>
            /// <returns></returns>
            /// <param name="arrayType"></param>
            private int GetLowerBound(int dimension, Type arrayType)
            {
                return GetBound("GetLowerBound", dimension, arrayType);
            }


            //        private int getUpperBound(int dimension)
            //        {
            // Not used, as UpperBound is equal LowerBound+Length
            //            return getBound("GetUpperBound", dimension);
            //        }

            private int GetBound(string methodName, int dimension, Type arrayType)
            {
                var methodInfo = arrayType.GetMethod(methodName);
                var bound = (int)methodInfo.Invoke(_array, new object[] { dimension });
                return bound;
            }

            private ArrayInfo GetArrayInfo(Type arrayType)
            {
                // Caching is unacceptable, as an array of type string can have different bounds

                var info = new ArrayInfo();

                // Fill the dimension infos
                for (var dimension = 0; dimension < GetRank(arrayType); dimension++)
                {
                    var dimensionInfo = new DimensionInfo();
                    dimensionInfo.Length = GetLength(dimension, arrayType);
                    dimensionInfo.LowerBound = GetLowerBound(dimension, arrayType);
                    info.DimensionInfos.Add(dimensionInfo);
                }


                return info;
            }

            ///<summary>
            ///</summary>
            ///<returns></returns>
            public IEnumerable<int[]> GetIndexes()
            {
                if (_indexes == null)
                {
                    _indexes = new List<int[]>();
                    ForEach(AddIndexes);
                }

                foreach (var item in _indexes)
                {
                    yield return item;
                }
            }

            ///<summary>
            ///</summary>
            ///<returns></returns>
            public IEnumerable<object> GetValues()
            {
                foreach (var indexSet in GetIndexes())
                {
                    var value = ((Array)_array).GetValue(indexSet);
                    yield return value;
                }
            }

            private void AddIndexes(int[] obj)
            {
                _indexes.Add(obj);
            }


            ///<summary>
            ///</summary>
            ///<param name = "action"></param>
            public void ForEach(Action<int[]> action)
            {
                var dimensionInfo = _arrayInfo.DimensionInfos[0];
                for (var index = dimensionInfo.LowerBound; index < dimensionInfo.LowerBound + dimensionInfo.Length; index++)
                {
                    var result = new List<int>();

                    // Adding the first coordinate
                    result.Add(index);

                    if (_arrayInfo.DimensionInfos.Count < 2)
                    {
                        // only one dimension
                        action.Invoke(result.ToArray());
                        continue;
                    }

                    // further dimensions
                    ForEach(_arrayInfo.DimensionInfos, 1, result, action);
                }
            }


            /// <summary>
            ///   This function will be recursively used
            /// </summary>
            /// <param name = "dimensionInfos"></param>
            /// <param name = "dimension"></param>
            /// <param name = "coordinates"></param>
            /// <param name = "action"></param>
            private void ForEach(IList<DimensionInfo> dimensionInfos, int dimension, IEnumerable<int> coordinates,
                                 Action<int[]> action)
            {
                var dimensionInfo = dimensionInfos[dimension];
                for (var index = dimensionInfo.LowerBound; index < dimensionInfo.LowerBound + dimensionInfo.Length; index++)
                {
                    var result = new List<int>(coordinates);

                    // Adding the first coordinate
                    result.Add(index);

                    if (dimension == _arrayInfo.DimensionInfos.Count - 1)
                    {
                        // This is the last dimension
                        action.Invoke(result.ToArray());
                        continue;
                    }

                    // Further dimensions
                    ForEach(_arrayInfo.DimensionInfos, dimension + 1, result, action);
                }
            }
        }

    /// <summary>
    ///   Contain info about array (i.e. how many dimensions, lower/upper bounds)
    /// </summary>
    internal sealed class ArrayInfo
        {
            private IList<DimensionInfo> _dimensionInfos;

            ///<summary>
            ///</summary>
            public IList<DimensionInfo> DimensionInfos
            {
                get
                {
                    if (_dimensionInfos == null) _dimensionInfos = new List<DimensionInfo>();
                    return _dimensionInfos;
                }
                set { _dimensionInfos = value; }
            }
        }
}