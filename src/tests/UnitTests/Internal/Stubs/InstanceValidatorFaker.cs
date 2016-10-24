using Moq;
using SafeOrbit.Memory;
using SafeOrbit.Memory.SafeContainerServices.Instance.Validation;
using SafeOrbit.Tests;

namespace SafeOrbit.Internal.Stubs
{
    internal class InstanceValidatorFaker : StubProviderBase<IInstanceValidator>
    {
        public override IInstanceValidator Provide() => Mock.Of<IInstanceValidator>();
    }
}