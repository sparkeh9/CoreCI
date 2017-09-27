namespace CoreCI.BuildAgent.Common.Implementation
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using CoreCI.Common.Models;
    using CoreCI.Common.Models.Jobs;
    using Models.BuildFile;
    using Polly;
    using Polly.Retry;
    using Sdk;

    public class BuildAgentDaemon : IBuildAgentDaemon
    {
        private readonly IBuildFileParser buildFileParser;

        private readonly ICoreCI coreCiClient;
        private readonly DockerBuildProcessor dockerBuildProcessor;
        private readonly NativeBuildProcessor nativeBuildProcessor;
        private readonly IBuildProgressReporter progressReporter;
        private readonly IVcsAppropriator vcsAppropriator;
        private readonly RetryPolicy waitForeverPolicy;
        private IBuildProcessor buildProcessor;

        private bool isRegistered;

        public BuildAgentDaemon( ICoreCI coreCiClient, IVcsAppropriator vcsAppropriator, IBuildFileParser buildFileParser, IBuildProgressReporter progressReporter, DockerBuildProcessor dockerBuildProcessor,
                                 NativeBuildProcessor nativeBuildProcessor )
        {
            this.coreCiClient = coreCiClient;
            this.vcsAppropriator = vcsAppropriator;
            this.buildFileParser = buildFileParser;
            this.progressReporter = progressReporter;
            this.dockerBuildProcessor = dockerBuildProcessor;
            this.nativeBuildProcessor = nativeBuildProcessor;

            this.vcsAppropriator.OnProgress += ( sender, report ) => { progressReporter.ReportAsync( report ); };
            waitForeverPolicy = Policy.Handle<Exception>()
                                      .WaitAndRetryForeverAsync( Sleep, onRetry : async ( exception, span ) => { await OnRetryAsync( exception, span ); } );
        }

        public event EventHandler<bool> PollStatusChanged;

        public async Task InvokeAsync( BuildEnvironment environment )
        {
            try
            {
                DisablePolling();

                if ( !isRegistered )
                {
                    await waitForeverPolicy.ExecuteAsync( async () =>
                                                          {
                                                              await progressReporter.ReportAsync( new JobProgressDto( "Attempting to register build agent with coordinator" ) );
                                                              await coreCiClient.Agents.RegisterAsync( environment );
                                                              await progressReporter.ReportAsync( new JobProgressDto( "Registered with coordinator" ) );
                                                              isRegistered = true;
                                                          } );
                }

                await progressReporter.ReportAsync( new JobProgressDto( "Checking for available jobs", JobProgressType.Command ) );

                var job = await coreCiClient.Jobs.ReserveFirstAvailableJobAsync( environment );

                if ( !job.HasValue )
                {
                    EnablePolling();
                    return;
                }

                string tempPath = GenerateTempPath();

                await progressReporter.ReportAsync( new JobProgressDto
                {
                    Message = $"Reserved job {job.Value.job.JobId}",
                    JobProgressType = JobProgressType.Informational
                } );

                await vcsAppropriator.AcquireAsync( job.Value.job, tempPath );
                var buildFile = buildFileParser.ParseBuildFile( tempPath );
                InitialiseBuildProcessor( buildFile );
                await buildProcessor.DoBuildAsync( job.Value.job, buildFile, tempPath );
            }
            catch ( Exception e )
            {
                await progressReporter.ReportAsync( new JobProgressDto
                {
                    Message = e.Message,
                    JobProgressType = JobProgressType.Error
                } );
            }
            finally
            {
                if ( buildProcessor != null )
                {
                    buildProcessor.OnProgress -= BuildProcessorOnProgress;
                }
                EnablePolling();
            }
        }

        public async Task StopAsync()
        {
            DisablePolling();

            if ( !isRegistered )
            {
                return;
            }

            try
            {
                await coreCiClient.Agents.DeregisterAsync();
            }
            catch ( Exception e )
            {
                Console.WriteLine( e.Message );
            }
        }

        private void InitialiseBuildProcessor( BuildFile buildFile )
        {
            if ( buildFile.BuildMode == BuildMode.Native )
            {
                buildProcessor = nativeBuildProcessor;
            }
            else
            {
                buildProcessor = dockerBuildProcessor;
            }

            buildProcessor.OnProgress -= BuildProcessorOnProgress;
            buildProcessor.OnProgress += BuildProcessorOnProgress;
        }

        private void BuildProcessorOnProgress( object o, JobProgressDto report )
        {
            progressReporter.ReportAsync( report );
        }

        private async Task OnRetryAsync( Exception exception, TimeSpan calculatedWaitDuration )
        {
            DisablePolling();
            await progressReporter.ReportAsync( new JobProgressDto
            {
                Message = $"{exception.Message} - retrying in {calculatedWaitDuration}",
                JobProgressType = JobProgressType.Error
            } );
        }

        private static TimeSpan Sleep( int attempt )
        {
            return TimeSpan.FromSeconds( 30 );
        }

        private void DisablePolling()
        {
            PollStatusChanged?.Invoke( this, false );
        }

        private void EnablePolling()
        {
            PollStatusChanged?.Invoke( this, true );
        }

        private static string GenerateTempPath()
        {
            return Path.GetFullPath( string.Join( $"{Path.DirectorySeparatorChar}", new[]
            {
                Path.GetTempPath(),
                "coreci",
                Guid.NewGuid().ToString( "D" )
            } ) );
        }
    }
}
