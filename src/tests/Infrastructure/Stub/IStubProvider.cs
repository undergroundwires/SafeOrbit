using System;

namespace SafeOrbit.Tests
{
    public interface IStubProvider<out T>  : IStubProvider
    {
        new T Provide();
    }
    public interface IStubProvider
    {
        object Provide();
    }
}