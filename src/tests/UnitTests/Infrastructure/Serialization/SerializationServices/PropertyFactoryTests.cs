﻿using System.Collections.Generic;
using NUnit.Framework;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Advanced;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Core;
using SafeOrbit.Infrastructure.Serialization.SerializationServices.Serializing;

namespace SafeOrbit.Infrastructure.Serialization.SerializationServices
{
    [TestFixture]
    public class PropertyFactoryTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [Test]
        public void Test_List1AndList1AsPropertiesAtSameDepth()
        {
            var list1 = new List<object>();
            var root = new Root(list1, list1);

            var factory = new PropertyFactory(new PropertyProvider());
            Property p = factory.CreateProperty("Root", root);

            // Always 2 properties in Root
            var complexProperty = p as ComplexProperty;
            Assert.IsNotNull(complexProperty);
            Assert.AreEqual(2, complexProperty.Properties.Count);
            Assert.AreEqual(1, complexProperty.Reference.Count);
            Assert.IsFalse(complexProperty.Reference.IsProcessed);

            // Both lists are of CollectionProperty
            var lp1 = complexProperty.Properties[0] as CollectionProperty;
            Assert.IsNotNull(lp1);
            var lp2 = complexProperty.Properties[1] as CollectionProperty;
            Assert.IsNotNull(lp2);

            Assert.AreEqual(lp1.Reference, lp2.Reference);
            Assert.AreEqual(2, lp1.Reference.Count);
        }

        [Test]
        public void Test_List1ContainsList2()
        {
            var list1 = new List<object>();
            var list2 = new List<object>();
            var root = new Root(list1, list2);
            list1.Add(list2);

            var factory = new PropertyFactory(new PropertyProvider());
            Property p = factory.CreateProperty("Root", root);

            // Always 2 properties in Root
            var complexProperty = p as ComplexProperty;
            Assert.IsNotNull(complexProperty);
            Assert.AreEqual(2, complexProperty.Properties.Count);
            Assert.AreEqual(1, complexProperty.Reference.Count);
            Assert.IsFalse(complexProperty.Reference.IsProcessed);

            // Both lists are of CollectionProperty
            var lp1 = complexProperty.Properties[0] as CollectionProperty;
            Assert.IsNotNull(lp1);
            var lp2 = complexProperty.Properties[1] as CollectionProperty;
            Assert.IsNotNull(lp2);

            Assert.AreEqual(1, lp1.Items.Count);
            var lp1i1 = lp1.Items[0] as CollectionProperty;
            Assert.IsNotNull(lp1i1);
            Assert.AreNotEqual(lp1i1, lp2);

            Assert.AreEqual(lp1i1.Reference, lp2.Reference);
            Assert.AreEqual(2, lp1i1.Reference.Count);
        }

        [Test]
        public void Test_List1ContainsList3_List2ContainsList3()
        {
            var list1 = new List<object>();
            var list2 = new List<object>();
            var list3 = new List<object>();
            var root = new Root(list1, list2);
            list1.Add(list3);
            list2.Add(list3);

            var factory = new PropertyFactory(new PropertyProvider());
            Property p = factory.CreateProperty("Root", root);

            var rootProperty = (ComplexProperty)p;
            // Both lists are of CollectionProperty
            var lp1 = (CollectionProperty)rootProperty.Properties[0];
            var lp2 = (CollectionProperty)rootProperty.Properties[1];

            var lp1i1 = (CollectionProperty)lp1.Items[0];
            var lp2i1 = (CollectionProperty)lp2.Items[0];
            Assert.AreNotEqual(lp1i1, lp2i1);

            Assert.AreEqual(lp1i1.Reference, lp2i1.Reference);
            Assert.AreEqual(2, lp1i1.Reference.Count);
        }

        private class Root
        {
            public Root(List<object> list1, List<object> list2)
            {
                List1 = list1;
                List2 = list2;
            }

            public List<object> List1 { get; set; }
            public List<object> List2 { get; set; }
        }
    }
}