namespace CoreCI.Common.Models.Jobs
{
    using Vcs;

    public class BitBucketVcsJob : GitVcsJob
    {
        public override VcsType VcsType => VcsType.BitBucketGit;
        public OAuth2Credentials OAuth2Credentials { get; set; }
    }
}
