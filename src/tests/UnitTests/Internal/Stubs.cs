using System.Reflection;
using Moq;
using SafeOrbit.Tests;

namespace SafeOrbit.Fakes
{
    public class Stubs
    {
        private static StubFactory _factory;

        public static T Get<T>()
        {
            if (_factory == null) _factory = GetFactory();
            return _factory.Provide<T>();
        }

        public static IFactory<T> GetFactory<T>(T singletonInstance = null)
            where T : class
        {
            var mock = new Mock<IFactory<T>>();
            mock.Setup(f => f.Create())
                .Returns(() => singletonInstance ?? Get<T>());
            return mock.Object;
        }

        private static StubFactory GetFactory()
        {
            var factory = new StubFactory();
            var currentAssembly = typeof(Stubs).GetTypeInfo().Assembly;
            factory.RegisterAll(currentAssembly);
            return factory;
        }
    }
}