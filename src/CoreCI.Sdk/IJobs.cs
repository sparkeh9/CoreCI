namespace CoreCI.Sdk
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Models;
    using Common.Models.Jobs;
    using MongoDB.Bson;

    public interface IJobs
    {
        Task<IEnumerable<JobDto>> ListAvailableJobsAsync( BuildEnvironment environment );
        Task<(JobDto job, JobReservedDto reservation)?> ReserveFirstAvailableJobAsync( BuildEnvironment environment );
        Task<JobDto> GetJobDetailsAsync( ObjectId jobIdt );
        Task<JobReservedDto> ReserveJobAsync( ObjectId jobId );
        Task<JobDto> FindByIdAsync( ObjectId jobId );
    }
}
