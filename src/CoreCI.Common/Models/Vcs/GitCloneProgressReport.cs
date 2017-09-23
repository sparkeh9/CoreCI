namespace CoreCI.Common.Models.Vcs
{
    using ByteSizeLib;

    public class GitCloneProgressReport
    {
        public int ObjectsReceived { get; set; }
        public int TotalObjects { get; set; }
        public long BytesReceived { get; set; }

        public override string ToString()
        {
            var size = ByteSize.FromBytes( BytesReceived );
            return size.ToString();
        }
    }
}
