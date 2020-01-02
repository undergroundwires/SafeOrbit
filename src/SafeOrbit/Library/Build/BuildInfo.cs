namespace SafeOrbit.Library.Build
{
    public class BuildInfo : IBuildInfo
    {
        public Platform TargetPlatform { get; } = GetTargetPlatform();
        public BuildMode BuildMode { get; } = GetBuildMode();


        private static Platform GetTargetPlatform()
        {
            // Different preprocessor symbols: https://docs.microsoft.com/en-us/dotnet/core/tutorials/libraries
#if NETSTANDARD1_6
            return Platform.NetStandard1_6;
#elif NETSTANDARD2_0
            return Platform.NetStandard2;
#elif NET472
            return Platform.Net472;
#elif NET471
            return Platform.Net471;
#elif NET47
            return Platform.Net470;
#elif NET462
            return Platform.Net462;
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