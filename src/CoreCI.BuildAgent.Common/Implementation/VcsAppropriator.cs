namespace CoreCI.BuildAgent.Common.Implementation
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using CoreCI.Common.Models;
    using CoreCI.Common.Models.Jobs;
    using CoreCI.Common.Vcs.Git;
    using Models;

    public class VcsAppropriator : IVcsAppropriator
    {
        public event EventHandler<JobProgressDto> OnProgress;

        public async Task AcquireAsync( JobDto job, string path )
        {
            await Task.Delay( 0 );
            switch ( job.Data.VcsType )
            {
                case VcsType.Git:
                    if ( !( job.Data is GitVcsJob gitVcsJob ) )
                        throw new ArgumentOutOfRangeException( nameof( job.Data ) );

                    AcquireGenericGit( job, gitVcsJob, path );
                    return;
                case VcsType.BitBucketGit:
                    if ( !( job.Data is BitBucketGitVcsJob bitbucketGitVcsJob ) )
                        throw new ArgumentOutOfRangeException( nameof( job.Data ) );

                    AcquireBitBucketGit( job, bitbucketGitVcsJob, path );
                    break;
                default:
                    throw new ArgumentOutOfRangeException( nameof( job.Data.VcsType ) );
            }

            CleanDirectoryRecursively( new DirectoryInfo( path ) );
        }

        private void AcquireGenericGit( JobDto jobDto, GitVcsJob jobData, string path )
        {
            var genericGitProvider = new GenericGitVcsProvider( jobData.BasicAuthenticationCredentials );
            genericGitProvider.OnProgress += ( sender, report ) => OnProgress?.Invoke( this, new JobProgressDto
            {
                ProgressType = ProgressType.Informational,
                Message = report.ToString()
            } );
            genericGitProvider.Clone( jobData.Url, $"{path}{Path.DirectorySeparatorChar}" );
        }

        private void AcquireBitBucketGit( JobDto jobDto, BitBucketGitVcsJob jobData, string path )
        {
            var bitbucketGitProvider = jobData.OAuth2Credentials != null
                ? new BitBucketGitVcsProvider( jobData.OAuth2Credentials )
                : new BitBucketGitVcsProvider( jobData.BasicAuthenticationCredentials );

            bitbucketGitProvider.OnProgress += ( sender, report ) => OnProgress?.Invoke( this, new JobProgressDto
            {
                ProgressType = ProgressType.Informational,
                Message = report.ToString()
            } );

            OnProgress?.Invoke( this, new JobProgressDto
            {
                ProgressType = ProgressType.Command,
                Message = $"Cloning {jobData.Url} into {path}"
            } );
            bitbucketGitProvider.Clone( jobData.Url,path );
        }

        private static void CleanDirectoryRecursively( DirectoryInfo directory )
        {
            foreach ( var subDirectory in directory.GetDirectories() )
                CleanDirectoryRecursively( subDirectory );

            directory.Attributes = FileAttributes.Normal;

            foreach ( var file in directory.GetFiles() )
                file.Attributes = FileAttributes.Normal;
        }
    }
}
