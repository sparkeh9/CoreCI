namespace CoreCI.BuildAgent.Common.Models
{
    using System;

    public class JobProgressDto
    {
        public string BuildAgentToken { get; set; }
        public string Message { get; set; }
        public ProgressType ProgressType { get; set; } = ProgressType.Informational;
        public DateTime ReportedAt { get; } = DateTime.UtcNow;
        public bool Recorded { get; set; } = true;

        public JobProgressDto() { }

        public JobProgressDto( string message, ProgressType progressType = ProgressType.Informational )
        {
            Message = message;
            ProgressType = progressType;
        }
    }

    public enum ProgressType
    {
        Command,
        Informational,
        Warning,
        Success,
        Error
    }
}
