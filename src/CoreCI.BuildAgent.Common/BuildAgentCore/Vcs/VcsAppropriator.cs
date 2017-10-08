namespace CoreCI.BuildAgent.Common.BuildAgentCore.Vcs
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using CoreCI.Common.Models;
    using CoreCI.Common.Models.Jobs;
    using CoreCI.Common.Vcs.Git;
    using Konsole;

    public class VcsAppropriator : IVcsAppropriator
    {
        private ProgressBar progressBar;
        public event EventHandler<JobProgressDto> OnProgress;

        public async Task AcquireAsync( JobDto job, string path )
        {
            await Task.Delay( 0 );
            switch ( job.Data.VcsType )
            {
                case VcsType.Git:
                    if ( !( job.Data is GitVcsJob gitVcsJob ) )
                    {
                        throw new ArgumentOutOfRangeException( nameof( job.Data ) );
                    }

                    AcquireGenericGit( gitVcsJob, path );
                    return;
                case VcsType.BitBucketGit:
                    if ( !( job.Data is BitBucketGitVcsJob bitbucketGitVcsJob ) )
                    {
                        throw new ArgumentOutOfRangeException( nameof( job.Data ) );
                    }

                    AcquireBitBucketGit( bitbucketGitVcsJob, path );
                    break;
                default:
                    throw new ArgumentOutOfRangeException( nameof( job.Data.VcsType ) );
            }

            CleanDirectoryRecursively( new DirectoryInfo( path ) );
        }

        private void AcquireGenericGit( GitVcsJob jobData, string path )
        {
            var lastUpdated = DateTime.Now.AddSeconds( -1 );
            var genericGitProvider = new GenericGitVcsProvider( jobData.BasicAuthenticationCredentials );
            genericGitProvider.OnProgress += ( sender, report ) =>
                                             {
                                                 if ( DateTime.Now - lastUpdated <= TimeSpan.FromSeconds( 0.5 ) )
                                                 {
                                                     return;
                                                 }

                                                 lastUpdated = DateTime.Now;
                                                 if ( progressBar == null )
                                                 {
                                                     progressBar = new ProgressBar( report.TotalObjects );
                                                 }

                                                 progressBar.Max = report.TotalObjects;
                                                 progressBar.Refresh( report.ObjectsReceived, report.ToString() );
                                             };

            OnProgress?.Invoke( this, new JobProgressDto
            {
                JobProgressType = JobProgressType.Command,
                Message = $"Cloning {jobData.Url} into {path}"
            } );
            genericGitProvider.Clone( jobData.Url, $"{path}{Path.DirectorySeparatorChar}" );
            progressBar = null;
        }

        private void AcquireBitBucketGit( BitBucketGitVcsJob jobData, string path )
        {
            progressBar = new ProgressBar( 100 );
            var bitbucketGitProvider = jobData.OAuth2Credentials != null
                ? new BitBucketGitVcsProvider( jobData.OAuth2Credentials )
                : new BitBucketGitVcsProvider( jobData.BasicAuthenticationCredentials );

            bitbucketGitProvider.OnProgress += ( sender, report ) =>
                                               {
                                                   if ( progressBar == null )
                                                   {
                                                       progressBar = new ProgressBar( report.TotalObjects );
                                                   }
                                                   progressBar.Max = report.TotalObjects;
                                                   progressBar.Refresh( report.ObjectsReceived, report.ToString() );
                                               };

            OnProgress?.Invoke( this, new JobProgressDto
            {
                JobProgressType = JobProgressType.Command,
                Message = $"Cloning {jobData.Url} into {path}"
            } );
            bitbucketGitProvider.Clone( jobData.Url, path );
            progressBar = null;
        }

        private static void CleanDirectoryRecursively( DirectoryInfo directory )
        {
            foreach ( var subDirectory in directory.GetDirectories() )
                CleanDirectoryRecursively( subDirectory );

            directory.Attributes = FileAttributes.Normal;

            foreach ( var file in directory.GetFiles() )
            {
                file.Attributes = FileAttributes.Normal;
            }
        }
    }
}
