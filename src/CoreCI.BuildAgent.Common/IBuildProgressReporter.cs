namespace CoreCI.BuildAgent.Common
{
    using System.Threading.Tasks;
    using Models;

    public interface IBuildProgressReporter
    {
        Task ReportAsync( JobProgressDto progressItem );
    }
}
