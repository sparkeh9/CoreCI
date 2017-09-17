namespace CoreCI.Common.Models.Jobs
{
    using Vcs;

    public class GitVcsJob : IVcsJob
    {
        public virtual VcsType VcsType => VcsType.Git;
        public string Url { get; set; }
        public BasicAuthenticationCredentials BasicAuthenticationCredentials { get; set; }
    }
}
