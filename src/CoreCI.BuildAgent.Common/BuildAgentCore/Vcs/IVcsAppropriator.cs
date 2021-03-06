﻿namespace CoreCI.BuildAgent.Common.BuildAgentCore.Vcs
{
    using System;
    using System.Threading.Tasks;
    using CoreCI.Common.Models.Jobs;

    public interface IVcsAppropriator
    {
        Task AcquireAsync( JobDto job, string path );
        event EventHandler<JobProgressDto> OnProgress;
    }
}
