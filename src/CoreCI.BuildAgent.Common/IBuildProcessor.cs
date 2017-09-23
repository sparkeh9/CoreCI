namespace CoreCI.BuildAgent.Common
{
    using System;
    using System.Threading.Tasks;
    using CoreCI.Common.Models.Jobs;
    using Models;
    using Models.BuildFile;

    public interface IBuildProcessor
    {
        Task DoBuildAsync( JobDto job, BuildFile buildFile, string path );
        event EventHandler<JobProgressDto> OnProgress;
    }
}
