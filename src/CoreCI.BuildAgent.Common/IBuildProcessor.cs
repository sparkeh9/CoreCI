namespace CoreCI.BuildAgent.Common
{
    using System;
    using CoreCI.Common.Models.Jobs;
    using Models;
    using Models.BuildFile;

    public interface IBuildProcessor
    {
        void DoBuild( JobDto job, BuildFile buildFile, string path );
        event EventHandler<JobProgressDto> OnProgress;
    }
}
