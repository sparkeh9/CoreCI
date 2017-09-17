namespace CoreCI.Common.Models.Vcs
{
    public class GitCloneProgressReport
    {
        public int ObjectsReceived { get; set; }
        public int TotalObjects { get; set; }
        public long BytesReceived { get; set; }
    }
}
