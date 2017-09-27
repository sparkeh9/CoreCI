namespace CoreCI.Web.Api.v1.Requests
{
    using Common.Models;

    public class GetJobsRequest
    {
        public int Page { get; set; } = 1;
        public BuildEnvironment? Environment { get; set; }
        public JobStatus JobStatus { get; set; } = JobStatus.Available;
    }
}
