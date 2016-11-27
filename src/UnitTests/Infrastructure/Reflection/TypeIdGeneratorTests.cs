
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

using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SafeOrbit.Tests;

namespace SafeOrbit.Infrastructure.Reflection
{
    /// <seealso cref="ITypeIdGenerator" />
    /// <seealso cref="TypeIdGeneratorTests" />
    [TestFixture]
    public class TypeIdGeneratorTests : TestsFor<ITypeIdGenerator>
    {
        protected override ITypeIdGenerator GetSut() => new TypeIdGenerator();

        [Test]
        public void AllClassesInSutAndTestAssemblies_HaveUniqueKeys()
        {
            //arrange
            var sutAssembly = typeof(TypeIdGenerator).GetTypeInfo().Assembly;
            var testAssembly = typeof(TypeIdGeneratorTests).GetTypeInfo().Assembly;
            var allTypes = sutAssembly.GetTypes().Concat(testAssembly.GetTypes());
            var expected = allTypes.Count();
            var sut = GetSut();
            //act
            var keys = allTypes.Select(type => sut.Generate(type));
            var actual = keys.Distinct().Count();
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Generate_ForSameTypes_ReturnsSame()
        {
            //arrange
            var sut = GetSut();
            var type = typeof(int);
            //act
            var expected = sut.Generate(type);
            var actual = sut.Generate(type);
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GenerateGeneric_ForSameTypes_AreSame()
        {
            //arrange
            var sut = GetSut();
            //act
            var expected = sut.Generate<DateTime>();
            var actual = sut.Generate<DateTime>();
            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test]
        public void Generate_ForDifferentTypes_ReturnsDifferent()
        {
            //arrange
            var sut = GetSut();
            var type = typeof(long);
            var differentType = typeof(int);
            //act
            var type1Result = sut.Generate(type);
            var type2Result = sut.Generate(differentType);
            //assert
            Assert.That(type1Result, Is.Not.EqualTo(type2Result));
        }

        [Test]
        public void GenerateGeneric_ForDifferentTypes_ReturnsDifferent()
        {
            //arrange
            var sut = GetSut();
            //act
            var type1Result = sut.Generate<long>();
            var type2Result = sut.Generate<int>();
            //assert
            Assert.That(type1Result, Is.Not.EqualTo(type2Result));
        }

        [Test]
        public void Generate_And_GenerateGeneric_ReturnsSame()
        {
            var sut = GetSut();
            //string case
            var expected1= sut.Generate<string>();
            var expected2 = sut.Generate<int>();
            var expected3 = sut.Generate<TypeIdGeneratorTests>();
            var actual1 = sut.Generate(typeof(string));
            var actual2 = sut.Generate(typeof(int));
            var actual3 = sut.Generate(typeof(TypeIdGeneratorTests));
            //assert
            Assert.That(expected1, Is.EqualTo(actual1));
            Assert.That(expected2, Is.EqualTo(actual2));
            Assert.That(expected3, Is.EqualTo(actual3));
        }

    }
}