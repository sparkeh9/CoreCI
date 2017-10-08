namespace CoreCI.BuildAgent.Common.BuildAgentCore.BuildFile
{
    using Models.BuildFile;

    public interface IBuildFileParser
    {
        BuildFile ParseBuildFile( string path );
    }
}
