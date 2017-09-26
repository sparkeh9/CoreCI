namespace CoreCI.BuildAgent.Common.Models.BuildFile
{
    public class DockerImage
    {
        public string Parent { get; set; }
        public string Tag { get; set; } = "latest";

        public override string ToString() => $"{Parent}:{Tag}";
    }
}
