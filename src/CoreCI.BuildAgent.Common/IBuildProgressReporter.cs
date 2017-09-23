namespace CoreCI.BuildAgent.Common
{
    using System.Threading.Tasks;
    using Models;

    public interface IBuildProgressReporter
    {
        Task ReportAsync( string buildAgentToken, JobProgressDto progressItem );
        Task ReportAsync( JobProgressDto progressItem );
    }
}
