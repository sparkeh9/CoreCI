namespace CoreCI.BuildAgent.Common.BuildAgentCore.Progress
{
    using CoreCI.Common.Models.Jobs;

    public class NullConsoleReporter : IConsoleProgressReporter
    {
        public void Report( JobProgressDto dto ) { }
    }
}
