
///*
//MIT License

//Copyright (c) 2016 Erkin Ekici - undergroundwires@safeorb.it

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//*/

//using System.Collections;
//using NUnit.Framework;
//using SafeOrbit.Tests;

//namespace SafeOrbit.Converters
//{
//    public class IntToBytesConverterTests : TestsFor<IntToBytesConverter>
//    {
//        protected override IntToBytesConverter GetSut()
//        {
//            return new IntToBytesConverter();
//        }

//        [Test]
//        [TestCaseSource(typeof (IntToBytesConverterTests), nameof(LittleEndianTestVectors))]
//        public byte[] Convert_Returns_LittleEndianTestVectors(int test)
//        {
//            var sut = GetSut();
//            var result = sut.Convert(test);
//            return result;
//        }

//        public static IEnumerable LittleEndianTestVectors
//        {
//            get
//            {
//                yield return new TestCaseData(32).Returns(new byte[] {32, 0, 0, 0});
//                yield return new TestCaseData(0).Returns(new byte[] {0, 0, 0, 0});
//                yield return new TestCaseData(-1).Returns(new byte[] {255, 255, 255, 255});
//                yield return new TestCaseData(555555554).Returns(new byte[] {226, 26, 29, 33});
//                yield return new TestCaseData(-200).Returns(new byte[] {56, 255, 255, 255});
//                yield return new TestCaseData(0x7fffffff).Returns(new byte[] {255, 255, 255, 127});
//                yield return new TestCaseData(-2147483648).Returns(new byte[] {0, 0, 0, 128});

//            }
//        }
//    }
//}
