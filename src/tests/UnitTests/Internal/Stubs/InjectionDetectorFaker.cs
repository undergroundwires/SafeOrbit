using Moq;
using SafeOrbit.Memory;
using SafeOrbit.Tests;

namespace SafeOrbit.UnitTests
{
    public class InjectionDetectorFaker : StubProviderBase<IInjectionDetector>
    {
        public override IInjectionDetector Provide()
        {
            return Mock.Of<IInjectionDetector>();
        }
    }
}