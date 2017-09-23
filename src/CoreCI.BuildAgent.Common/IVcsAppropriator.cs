namespace CoreCI.BuildAgent.Common
{
    using System;
    using System.Threading.Tasks;
    using CoreCI.Common.Models.Jobs;
    using Models;

    public interface IVcsAppropriator
    {
        Task AcquireAsync( JobDto job, string path );
        event EventHandler<JobProgressDto> OnProgress;
    }
}
