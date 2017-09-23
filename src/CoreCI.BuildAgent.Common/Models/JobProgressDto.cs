namespace CoreCI.BuildAgent.Common.Models
{
    public class JobProgressDto
    {
        public string Message { get; set; }
        public ProgressType ProgressType { get; set; } = ProgressType.Informational;

        public JobProgressDto() { }

        public JobProgressDto( string message )
        {
            Message = message;
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
