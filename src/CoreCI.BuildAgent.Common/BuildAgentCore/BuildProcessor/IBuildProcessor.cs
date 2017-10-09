namespace CoreCI.BuildAgent.Common.BuildAgentCore.BuildProcessor
{
    using System;
    using System.Threading.Tasks;
    using CoreCI.Common.Models.Jobs;
    using Models.BuildFile;

    public interface IBuildProcessor
    {
        Task<JobCompletionResult> DoBuildAsync( JobDto job, BuildFile buildFile, string path );
        event EventHandler<JobProgressDto> OnProgress;
    }
}
