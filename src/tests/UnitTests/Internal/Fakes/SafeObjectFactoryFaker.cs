using System;
using Moq;
using SafeOrbit.Exceptions;
using SafeOrbit.Memory;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    /// <seealso cref="ISafeObjectFactory" />
    public class SafeObjectFactoryFaker : StubProviderBase<ISafeObjectFactory>
    {
        public override ISafeObjectFactory Provide() => new FakeSafeObjectFactory();

        private class FakeSafeObjectFactory : ISafeObjectFactory
        {
            public ISafeObject<T> Get<T>(IInitialSafeObjectSettings settings) where T : class, new()
            {
                var mock = new Mock<ISafeObject<T>>();
                var isReadOnly = false;
                var obj = settings.InitialValue == null ? new T() : (T) settings.InitialValue;
                mock.Setup(m => m.AlertChannel).Returns(settings.AlertChannel);
                mock.Setup(m => m.IsReadOnly).Returns(() => isReadOnly);
                mock.Setup(m => m.MakeReadOnly()).Callback(() => isReadOnly = true);
                mock.Setup(m => m.CurrentProtectionMode).Returns(settings.ProtectionMode);
                mock.Setup(m => m.ApplyChanges(It.IsAny<Action<T>>())).Callback(
                    (Action<T> action) =>
                    {
                        if(isReadOnly)
                            throw new WriteAccessDeniedException($"{nameof(ISafeObject<T>.MakeReadOnly)} is called");
                        action.Invoke(obj);
                    });
                mock.SetupGet(m => m.Object).Returns(obj);
                return mock.Object;
            }
        }
    }
}