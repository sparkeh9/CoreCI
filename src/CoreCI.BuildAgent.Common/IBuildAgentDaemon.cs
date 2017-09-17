namespace CoreCI.BuildAgent.Common
{
    using System;
    using System.Threading.Tasks;
    using CoreCI.Common.Models;

    public interface IBuildAgentDaemon
    {
        Task InvokeAsync( BuildEnvironment environment );
        Task StopAsync();
        event EventHandler<bool> PollStatusChanged;
    }
}
