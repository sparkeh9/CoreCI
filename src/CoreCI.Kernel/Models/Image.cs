namespace CoreCI.Kernel.Models
{
    public class Image
    {
        public string Parent { get; set; }
        public string Tag { get; set; }

        public override string ToString() => $"{Parent}:{Tag}";
    }
}