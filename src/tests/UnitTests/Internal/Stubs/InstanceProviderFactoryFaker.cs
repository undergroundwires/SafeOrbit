using System;
using Moq;
using SafeOrbit.Memory;
using SafeOrbit.Memory.InjectionServices;
using SafeOrbit.Memory.SafeContainerServices.Instance;
using SafeOrbit.Memory.SafeContainerServices.Instance.Providers;
using SafeOrbit.Tests;

namespace SafeOrbit.Internal.Stubs
{
    internal class InstanceProviderFactoryFaker : StubProviderBase<IInstanceProviderFactory>
    {
        public override IInstanceProviderFactory Provide()
        {
            var mock = new Mock<IInstanceProviderFactory>();
            mock.Setup(
                    m =>
                        m.Get<InstanceTestClass>(
                            It.IsAny<LifeTime>(),
                            It.IsAny<InstanceProtectionMode>(),
                            It.IsAny<InjectionAlertChannel>()))
                .Returns<LifeTime, InstanceProtectionMode, InjectionAlertChannel>
                ((lifeTime, protectionMode, alertChannel) =>
                {
                    var mockProvider = new Mock<IInstanceProvider>();
                    mockProvider.Setup(p => p.LifeTime).Returns(lifeTime);
                    mockProvider.Setup(p => p.CurrentProtectionMode).Returns(protectionMode);
                    mockProvider.Setup(p => p.AlertChannel).Returns(alertChannel);
                    mockProvider.Setup(p => p.Provide()).Returns(new InstanceTestClass());
                    return mockProvider.Object;
                });
            return mock.Object;
        }
    }
}