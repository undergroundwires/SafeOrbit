
/*
MIT License

Copyright (c) 2018 Erkin Ekici - undergroundwires@safeorb.it

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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace SafeOrbit.Tests.Equality
{
    [TestFixture]
    public abstract class EqualityTestsBase<T> : TestsFor<T>, IEqualityTests where T:class
    {
        /// <summary>
        /// Gets suts thats are different than (unequal to) the one from <see cref="TestsFor{T}.GetSut()"/>.
        /// There is no need to give <see langword="null"/> instances as they'll be tested automatically.
        /// </summary>
        protected abstract IEnumerable<T> GetDifferentSuts();

        #region Equals
        ///<summary>
        /// A test for Equals : Instances are equal
        ///</summary>
        [Test]
        public void Equals_EqualInstances_ReturnsTrue()
        {
            foreach (var instance in AllDifferentInstances)
            {
                Assert.That(instance, Is.EqualTo(instance));
            }
        }
        ///<summary>
        /// A test for Equals : Instances are unequal
        ///</summary>
        [Test]
        public void Equals_UnequalInstances_returnsFalse()
        {
            var instance = GetSut();
            foreach (var unequalInstance in GetDifferentSuts())
            {
                const bool expected = false;
                var actual = instance.Equals(unequalInstance);
                Assert.That(actual, Is.EqualTo(expected));
            }
        }
        ///<summary>
        /// A test for Equals : Instances as <see cref="object"/> are equal
        ///</summary>
        [Test]
        public void Equals_EqualInstancesAsObjects_ReturnsTrue()
        {
            foreach (var instance in AllDifferentInstances)
            {
                const bool expected = true;
                var targets = new object[] { instance, instance };
                var actual = targets[0].Equals(targets[1]);
                Assert.That(actual, Is.EqualTo(expected));
            }
        }
        ///<summary>
        /// A test for Equals : Instances as <see cref="object"/> are unequal
        ///</summary>
        [Test]
        public void Equals_UnequalInstancesAsObjects_returnsFalse()
        {
            var instance = GetSut();
            foreach (var unequalInstance in GetDifferentSuts())
            {
                const bool expected = false;
                var actual = instance.Equals(unequalInstance);
                Assert.That(actual, Is.EqualTo(expected));
            }
        }
        ///<summary>
        /// A test for Equals : Parameter is null
        ///</summary>
        [Test]
        public void Equals_SingleInstanceIsNull_returnsFalse()
        {
            const bool expected = false;
            var targets = new[] { GetSut(), null };
            var actual = targets[0].Equals(targets[1]);
            Assert.That(actual, Is.EqualTo(expected));
        }
        ///<summary>
        /// A test for Equals object : Type inequality
        ///</summary>
        [Test]
        public void Equals_OneInstanceIsDifferentType_returnsTrue()
        {
            const bool expected = false;
            var targets = new object[] { GetSut(), 0d };
            var actual = targets[0].Equals(targets[1]);
            Assert.That(actual, Is.EqualTo(expected));
        }
        #endregion

        #region Inequality Operator (==)
        ///<summary>
        /// A test for operator == : Equality
        ///</summary>
        [Test]
        public void EqualityOperator_EqualInstances_returnsTrue()
        {
            foreach (var instance in AllDifferentInstances)
            {
                const bool expected = true;
                var actual = GetEqualityOperatorResult(instance, instance);
                Assert.That(actual, Is.EqualTo(expected));
            }

        }
        ///<summary>
        /// A test for operator == : Inequality
        ///</summary>
        [Test]
        public void EqualityOperator_UnequalInstances_returnsFalse()
        {
            var instance = GetSut();
            foreach (var unequalInstance in GetDifferentSuts())
            {
                const bool expected = false;
                var actual = GetEqualityOperatorResult(instance, unequalInstance);
                Assert.That(actual, Is.EqualTo(expected));
            }
        }
        /// <summary>
        ///A test for operator == : Null inequality
        ///</summary>
        [Test]
        public void EqualityOperator_SingleInstanceIsNull_returnsTrue()
        {
            const bool expected = false;
            var targets = new T[] { null, GetSut() };
            var actual = GetEqualityOperatorResult(targets[0], targets[1]);
            Assert.That(actual, Is.EqualTo(expected));
        }
        ///<summary>
        /// A test for operator == : Null equality
        ///</summary>
        [Test]
        public void EqualityOperator_BothInstancesAreNull_returnsTrue()
        {
            const bool expected = true;
            var targets = new T[] { null, null };
            var actual = GetEqualityOperatorResult(targets[0], targets[1]);
            Assert.That(actual, Is.EqualTo(expected));
        }
        #endregion

        #region Inequality Operator (!=)
        ///<summary>
        /// A test for operator != : Equality
        ///</summary>
        [Test]
        public void InequalityOperator_EqualInstances_returnsFalse()
        {
            foreach (var instance in AllDifferentInstances)
            {
                const bool expected = false;
                var actual = GetInequalityOperatorResult(instance, instance);
                Assert.That(actual, Is.EqualTo(expected));
            }
        }
        ///<summary>
        /// A test for operator != : Inequality
        ///</summary>
        [Test]
        public void InequalityOperator_UnequalInstances_returnsTrue()
        {
            var instance = GetSut();
            foreach (var unequalInstance in GetDifferentSuts())
            {
                const bool expected = true;
                var actual = GetInequalityOperatorResult(instance, unequalInstance);
                Assert.That(actual, Is.EqualTo(expected));
            }
        }
        ///<summary>
        /// A test for operator != : Null inequality
        ///</summary>
        [Test]
        public void InequalityOperator_SingleInstanceIsNull_returnsTrue()
        {
            const bool expected = true;
            var targets = new[] { null, GetSut() };
            var actual = GetInequalityOperatorResult(targets[0], targets[1]);
            Assert.That(actual, Is.EqualTo(expected));
        }


        ///<summary>
        /// A test for operator != : Null Equality
        ///</summary>
        [Test]
        public void InequalityOperator_BothInstancesAreNull_returnsTrue()
        {
            const bool expected = false;
            var targets = new T[] { null, null };
            var actual = GetInequalityOperatorResult(targets[0], targets[1]);
            Assert.That(actual, Is.EqualTo(expected));
        }

        #endregion

        #region GetHashCode
        /// <summary>
        /// A test for GetHashCode : Equality
        ///</summary>
        [Test]
        public void GetHashCode_ForEqualInstances_areEqual()
        {
            foreach (var instance in AllDifferentInstances)
            {
                Assert.That(instance, Is.EqualTo(instance));
            }
        }
        /// <summary>
        /// A test for GetHashCode : Inequality
        ///</summary>
        [Test]
        public void GetHashCode_ForUnequalInstances_areDifferent()
        {
            var instance = GetSut();
                foreach (var unequalInstance in GetDifferentSuts())
                {
                    Assert.That(instance, Is.Not.EqualTo(unequalInstance));
                }
            
        }
        #endregion

        private IEnumerable<T> AllDifferentInstances
        {
            get
            {
                yield return GetSut();
                foreach(var differentSut in GetDifferentSuts())
                {
                    yield return differentSut;
                }
            }
        }

     


        /// <summary>
        /// Gets the result of "a == b"
        /// Note that operator == can't be applied directly generic types in C#, use this method instead
        /// </summary>
        private static bool GetEqualityOperatorResult<TClass>(TClass a, TClass b)
        {
            // declare the parameters
            var paramA = Expression.Parameter(typeof(TClass), nameof(a));
            var paramB = Expression.Parameter(typeof(TClass), nameof(b));
            // add the parameters together
            var body = Expression.Equal(paramA, paramB);
            // compile it
            var invokeEqualityOperator = Expression.Lambda<Func<TClass, TClass, bool>>(body, paramA, paramB).Compile();
            // call it
            return invokeEqualityOperator(a, b);
        }
        /// <summary>
        /// Gets the result of "a =! b"
        /// Note that operator =! can't be applied directly generic types in C#, use this method instead
        /// </summary>
        private static bool GetInequalityOperatorResult<TClass>(TClass a, TClass b)
        {
            // declare the parameters
            var paramA = Expression.Parameter(typeof(TClass), nameof(a));
            var paramB = Expression.Parameter(typeof(TClass), nameof(b));
            // add the parameters together
            var body = Expression.NotEqual(paramA, paramB);
            // compile it
            var invokeInequalityOperator = Expression.Lambda<Func<TClass, TClass, bool>>(body, paramA, paramB).Compile();
            // call it
            return invokeInequalityOperator(a, b);
        }
    }

}