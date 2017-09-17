namespace CoreCI.Sdk
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Models;
    using Common.Models.Jobs;

   public interface IJobs
    {
        Task<IEnumerable<JobDto>> ListAvailableJobsAsync( BuildEnvironment environment);
        Task<JobDto> ReserveFirstAvailableJobAsync( BuildEnvironment environment);
    }
}
