namespace CoreCI.BuildAgent.Common
{
    using System.Threading.Tasks;
    using Models.BuildFile;
    using Models.Docker;

    public interface IDockerBuildScriptGenerator
    {
        Task GenerateBuildScript( BuildFile buildFile, DockerJobEnvironmentConfiguration dockerEnvironmentConfig );
    }
}
