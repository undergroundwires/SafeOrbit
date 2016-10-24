using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SafeOrbit.Common.Extensions
{
    /// <see cref="IEnumerableExtensions"/>
    [TestFixture]
    public class IEnumerableExtensionsTests
    {
        [Test]
        public void EmptyIfNull_CallerIsEnumerableNull_ReturnsEmptyArray()
        {
            //arrange
            var nullArray = (IEnumerable<object>)null;
            var expected = Enumerable.Empty<object>();
            //act
            var result = nullArray.EmptyIfNull();
            //assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void EmptyIfNull_CallerIsNotNull_ReturnsCaller()
        {
            //arrange
            var expected = new int[] { 5, 6, 7 };
            //act
            var result = expected.EmptyIfNull();
            //assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ForEach_CallerIsNull_DoesNotThrow()
        {
            //arrange
            string[] nullCaller = null;
            //act
            TestDelegate callingOnNull = () => nullCaller.ForEach(a => a = "abc");
            //assert
            Assert.DoesNotThrow(callingOnNull);
        }

        [Test]
        public void ForEach_ActionIsNull_throwsArgumentNullException()
        {
            //arrange
           var list = new [] {"a","b"};
            //act
            TestDelegate calingWithNullAction = () => list.ForEach(null);
            //assert
            Assert.That(calingWithNullAction, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ForEach_ForAllItems_ActionIsExecuted()
        {
            var expected = "4";

            CustomObject[] customObjects = new[] { new CustomObject("1"), new CustomObject("3"), new CustomObject("2") };

            customObjects.ForEach(x => x.Name = expected);

            Assert.That(customObjects, Has.All.Property(nameof(CustomObject.Name)).EqualTo(expected));
        }

        [Test]
        public void ForEach_AfterExecuted_HoldsFirstItemCorrect()
        {
            CustomObject[] customObjects = new[] { new CustomObject("1"), new CustomObject("3"), new CustomObject("2") };

            customObjects.ForEach(x => x.IsAlive = false);

            Assert.AreEqual("1", customObjects.First().Name);
        }

        [Test]
        public void ForEach_AfterExecuted_HoldsLastItemCorrect()
        {
            CustomObject[] customObjects = new[] { new CustomObject("1"), new CustomObject("3"), new CustomObject("2") };

            customObjects.ForEach(x => x.IsAlive = false);

            Assert.AreEqual("2", customObjects.Last().Name);
        }

        /// <summary>
        /// Test class
        /// </summary>
        private class CustomObject
        {
            public string Name { get; set; }
            public bool IsAlive { get; set; }


            public CustomObject(string name)
            {
                Name = name;
                IsAlive = true;
            }
        }
    }
}