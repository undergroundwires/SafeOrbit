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