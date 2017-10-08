namespace CoreCI.Web.Api.v1.Requests
{
    using System.Collections.Generic;
    using Common.Models;

    public class GetJobsRequest
    {
        public int Page { get; set; } = 1;
        public List<BuildEnvironment> Environments { get; set; }
        public JobStatus JobStatus { get; set; } = JobStatus.Available;
    }
}
