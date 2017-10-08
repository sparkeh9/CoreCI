namespace CoreCI.Sdk
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Models;
    using Common.Models.Jobs;
    using MongoDB.Bson;

    public interface IJobs
    {
        Task<IEnumerable<JobDto>> ListAvailableJobsAsync( IEnumerable<BuildEnvironment> environments );
        Task<(JobDto job, JobReservedDto reservation)?> ReserveFirstAvailableJobAsync( IEnumerable<BuildEnvironment> environments );
        Task<JobDto> GetJobDetailsAsync( ObjectId jobIdt );
        Task<JobReservedDto> ReserveJobAsync( ObjectId jobId );
        Task<JobDto> FindByIdAsync( ObjectId jobId );
        Task ReportAsync( JobProgressDto jobProgress );
    }
}
