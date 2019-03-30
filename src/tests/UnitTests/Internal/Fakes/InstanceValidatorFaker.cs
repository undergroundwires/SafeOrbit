using Moq;
using SafeOrbit.Memory.SafeContainerServices.Instance.Validation;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="IInstanceValidator" />
    internal class InstanceValidatorFaker : StubProviderBase<IInstanceValidator>
    {
        public override IInstanceValidator Provide() => Mock.Of<IInstanceValidator>();
    }
}