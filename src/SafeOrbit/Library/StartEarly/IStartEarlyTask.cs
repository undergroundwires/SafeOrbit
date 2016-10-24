namespace SafeOrbit.Library.StartEarly
{
    /// <summary>
    ///     Abstraction for a class to be run during the application start for more performance gain.
    /// </summary>
    internal interface IStartEarlyTask
    {
        void Prepare();
    }
}