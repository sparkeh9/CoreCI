namespace CoreCI.Common.Vcs.Git
{
    using System;
    using Extensions;
    using LibGit2Sharp;
    using Models.Vcs;

    public class GenericGitVcsProvider : IGitVcsProvider
    {
        protected Credentials GitCredentials { get; set; }

        public event EventHandler<GitCloneProgressReport> OnProgress;
        public GenericGitVcsProvider() { }

        public GenericGitVcsProvider( BasicAuthenticationCredentials credentials )
        {
            if ( credentials.Username.IsNullOrWhiteSpace() )
            {
                GitCredentials = new DefaultCredentials();
            }
            else
            {
                GitCredentials = new UsernamePasswordCredentials
                {
                    Username = credentials.Username,
                    Password = credentials.Password
                };
            }
        }

        public void Clone( string url, string path )
        {
            var options = new CloneOptions
            {
                CredentialsProvider = ( repoUrl, user, _ ) => GitCredentials,
                OnTransferProgress = OnTransferProgress
            };
            Repository.Clone( url, path, options );
        }

        private bool OnTransferProgress( TransferProgress progress )
        {
            OnProgress?.Invoke( this, new GitCloneProgressReport
            {
                ObjectsReceived = progress.ReceivedObjects,
                TotalObjects = progress.TotalObjects,
                BytesReceived = progress.ReceivedBytes
            } );
            return true;
        }
    }
}
