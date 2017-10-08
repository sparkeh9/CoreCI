namespace CoreCI.BuildAgent.Common.BuildAgentCore.BuildProcessor
{
    using System;
    using System.Threading.Tasks;
    using CoreCI.Common.Models;

    public interface IBuildAgentDaemon
    {
        Task InvokeAsync( BuildEnvironmentOs environment );
        Task StopAsync();
        event EventHandler<bool> PollStatusChanged;
    }
}
