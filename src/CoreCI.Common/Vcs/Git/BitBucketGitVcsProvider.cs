namespace CoreCI.Common.Vcs.Git
{
    using LibGit2Sharp;
    using Models.Vcs;

    public class BitBucketGitVcsProvider : GenericGitVcsProvider
    {
        public BitBucketGitVcsProvider( BasicAuthenticationCredentials credentials ) : base( credentials ) { }

        public BitBucketGitVcsProvider( OAuth2Credentials oauthCredentials )
        {
            GitCredentials = new UsernamePasswordCredentials
            {
                Username = "x-token-auth",
                Password = oauthCredentials.AccessToken
            };
        }
    }
}
