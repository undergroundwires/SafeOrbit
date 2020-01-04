namespace SafeOrbit.Library.Build
{
    /// <summary>
    ///     Information about how SafeOrbit was built.
    /// </summary>
    public interface IBuildInfo
    {
        Platform TargetPlatform { get; }
        BuildMode BuildMode { get; }
    }
}