
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

namespace SafeOrbit.Tests.Equality
{
    public interface IEqualityTests
    {
        void EqualityOperator_BothInstancesAreNull_returnsTrue();
        void EqualityOperator_EqualInstances_returnsTrue();
        void EqualityOperator_SingleInstanceIsNull_returnsTrue();
        void EqualityOperator_UnequalInstances_returnsFalse();
        void Equals_EqualInstancesAsObjects_ReturnsTrue();
        void Equals_EqualInstances_ReturnsTrue();
        void Equals_OneInstanceIsDifferentType_returnsTrue();
        void Equals_SingleInstanceIsNull_returnsFalse();
        void Equals_UnequalInstancesAsObjects_returnsFalse();
        void Equals_UnequalInstances_returnsFalse();
        void GetHashCode_ForEqualInstances_areEqual();
        void GetHashCode_ForUnequalInstances_areDifferent();
        void InequalityOperator_BothInstancesAreNull_returnsTrue();
        void InequalityOperator_EqualInstances_returnsFalse();
        void InequalityOperator_SingleInstanceIsNull_returnsTrue();
        void InequalityOperator_UnequalInstances_returnsTrue();
    }
}