namespace CoreCI.BuildAgent.Common
{
    using System;
    using CoreCI.Common.Models.Jobs;
    using Models.BuildFile;

    public interface IBuildProcessor
    {
        void DoBuild( JobDto job, BuildFile buildFile, string path );
        event EventHandler<string> OnProgress;
    }
}
