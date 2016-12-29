namespace SafeOrbit.Library.Build
{
    public class BuildInfo : IBuildInfo
    {
        public Platform TargetPlatform { get; } = GetTargetPlatform();
        public BuildMode BuildMode { get; } = GetBuildMode();


        private static Platform GetTargetPlatform()
        {
#if NETCORE
            return Platform.NetCore;
#elif NET461
            return Platform.Net461;
#elif NET46
            return Platform.Net460;
#elif NET452
            return Platform.Net452;
#elif NET451
            return Platform.Net451;
#elif NET45
            return Platform.Net450;
#endif
        }

        private static BuildMode GetBuildMode()
        {
#if DEBUG
            return BuildMode.Debug;
#else
            return BuildMode.Release;
#endif
        }
    }
}