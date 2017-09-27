namespace CoreCI.Common.Models.Jobs
{
    using System;

    public class JobProgressDto
    {
        public string BuildAgentToken { get; set; }
        public string Message { get; set; }
        public JobProgressType JobProgressType { get; set; } = JobProgressType.Informational;
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
        public bool Recorded { get; set; } = true;

        public JobProgressDto() { }

        public JobProgressDto( string message, JobProgressType jobProgressType = JobProgressType.Informational )
        {
            Message = message;
            JobProgressType = jobProgressType;
        }
    }
}
