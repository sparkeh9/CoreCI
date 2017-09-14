namespace CoreCI.BuildAgent.Common
{
    using System;
    using System.Threading.Tasks;

    public interface IBuildAgentDaemon
    {
        Task InvokeAsync();
        Task StopAsync();
        event EventHandler<bool> PollStatusChanged;
    }
}
