namespace SafeOrbit
{
    /// <summary>
    ///     Generic factory for instances of type <typeparamref name="TComponent" />
    /// </summary>
    /// <typeparam name="TComponent">The type of the instance.</typeparam>
    public interface IFactory<out TComponent>
    {
        /// <summary>
        ///     Creates an instance of type <typeparamref name="TComponent" />
        /// </summary>
        /// <returns>An instance of type <typeparamref name="TComponent" /></returns>
        TComponent Create();
    }
}