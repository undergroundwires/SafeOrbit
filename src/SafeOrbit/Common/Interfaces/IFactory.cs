namespace SafeOrbit.Interfaces
{
    /// <summary>
    /// Generic factory for instances of type <see cref="TComponent"/>
    /// </summary>
    /// <typeparam name="TComponent">The type of the instance.</typeparam>
    public interface IFactory<out TComponent>
    {
        /// <summary>
        /// Creates an instance of type <see cref="TComponent"/>
        /// </summary>
        /// <returns>An instance of type <see cref="TComponent"/></returns>
        TComponent Create();
    }
}