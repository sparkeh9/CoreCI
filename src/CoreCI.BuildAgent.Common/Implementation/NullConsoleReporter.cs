namespace CoreCI.BuildAgent.Common.Implementation
{
    using CoreCI.Common.Models.Jobs;

    public class NullConsoleReporter : IConsoleProgressReporter
    {
        public void Report( JobProgressDto dto ) { }
    }
}
