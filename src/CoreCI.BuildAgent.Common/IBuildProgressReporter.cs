namespace CoreCI.BuildAgent.Common
{
    using System.Threading.Tasks;
    using CoreCI.Common.Models.Jobs;

    public interface IBuildProgressReporter
    {
        Task ReportAsync( JobProgressDto progressItem );
        void UseBuildAgentToken( string token );
    }
}
