namespace CoreCI.BuildAgent.Common
{
    using System.Threading.Tasks;
    using Models.BuildFile;

    public interface IBuildFileParser
    {
        BuildFile ParseBuildFile( string path );
    }
}
