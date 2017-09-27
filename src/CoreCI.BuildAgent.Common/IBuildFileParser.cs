namespace CoreCI.BuildAgent.Common
{
    using Models.BuildFile;

    public interface IBuildFileParser
    {
        BuildFile ParseBuildFile( string path );
    }
}
