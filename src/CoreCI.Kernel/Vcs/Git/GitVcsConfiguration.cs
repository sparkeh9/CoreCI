namespace CoreCI.Kernel.Vcs.Git
{
    public class GitVcsConfiguration : IVcsConfiguration
    {
        public string WorkspacePath { get; set; }
        public string RemoteUrl { get; set; }
        public string Sha { get; set; }
        public string Ref { get; set; }
    }
}