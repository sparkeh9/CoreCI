namespace CoreCI.BuildAgent.Common
{
    using CoreCI.Common.Models.Jobs;

    public interface IConsoleProgressReporter
    {
        void Report( JobProgressDto dto );
    }
}
