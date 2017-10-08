namespace CoreCI.BuildAgent.Common.BuildAgentCore.Progress
{
    using CoreCI.Common.Models.Jobs;

    public interface IConsoleProgressReporter
    {
        void Report( JobProgressDto dto );
    }
}
