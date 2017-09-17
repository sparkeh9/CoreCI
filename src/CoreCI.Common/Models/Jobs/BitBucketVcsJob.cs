namespace CoreCI.Common.Models.Jobs
{
    using Vcs;

    public class BitBucketVcsJob : GitVcsJob
    {
        public OAuth2Credentials OAuth2Credentials { get; set; }
    }
}
