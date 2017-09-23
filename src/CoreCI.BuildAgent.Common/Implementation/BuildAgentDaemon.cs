namespace CoreCI.BuildAgent.Common.Implementation
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using CoreCI.Common.Models;
    using Exceptions;
    using Models;
    using Polly;
    using Polly.Retry;
    using Sdk;

    public class BuildAgentDaemon : IBuildAgentDaemon
    {
        private readonly ICoreCI coreCiClient;
        private readonly IVcsAppropriator vcsAppropriator;
        private readonly IBuildFileParser buildFileParser;
        private readonly IBuildProcessor buildProcessor;
        private readonly IBuildProgressReporter progressReporter;

        private readonly RetryPolicy waitForeverPolicy;
        public event EventHandler<bool> PollStatusChanged;

        private bool isRegistered;

        public BuildAgentDaemon( ICoreCI coreCiClient, IVcsAppropriator vcsAppropriator, IBuildFileParser buildFileParser, IBuildProcessor buildProcessor, IBuildProgressReporter progressReporter )
        {
            this.coreCiClient = coreCiClient;
            this.vcsAppropriator = vcsAppropriator;
            this.buildFileParser = buildFileParser;
            this.buildProcessor = buildProcessor;
            this.progressReporter = progressReporter;

            this.vcsAppropriator.OnProgress += ( sender, report ) => { progressReporter.ReportAsync( "buildAgentToken", report ); };
            this.buildProcessor.OnProgress += ( sender, report ) => { progressReporter.ReportAsync( "buildAgentToken", report ); };

            waitForeverPolicy = Policy.Handle<Exception>()
                                      .WaitAndRetryForeverAsync( Sleep, onRetry : async ( exception, span ) => { await OnRetryAsync( exception, span ); } );
        }

        public async Task InvokeAsync( BuildEnvironment environment )
        {
            DisablePolling();

            if ( !isRegistered )
                await waitForeverPolicy.ExecuteAsync( async () =>
                                                      {
                                                          await progressReporter.ReportAsync( new JobProgressDto( "Attempting to register build agent with coordinator" ) );
                                                          await coreCiClient.Agents.RegisterAsync( environment );
                                                          isRegistered = true;
                                                      } );

            await progressReporter.ReportAsync( new JobProgressDto( "Checking for available jobs" ) );

            var job = await coreCiClient.Jobs.ReserveFirstAvailableJobAsync( environment );

            if ( !job.HasValue )
            {
                EnablePolling();
                return;
            }

            try
            {
                string tempPath = GenerateTempPath();

                await progressReporter.ReportAsync( new JobProgressDto
                {
                    Message = $"Reserved job {job.Value.job.JobId}",
                    ProgressType = ProgressType.Informational
                } );
                await vcsAppropriator.AcquireAsync( job.Value.job, tempPath );

                var buildFile = buildFileParser.ParseBuildFile( tempPath );
                buildProcessor.DoBuild( job.Value.job, buildFile, tempPath );
            }
            catch ( NoBuildFileFoundException e )
            {
                await progressReporter.ReportAsync( new JobProgressDto
                {
                    Message = e.Message,
                    ProgressType = ProgressType.Error
                } );
            }
            catch ( Exception e )
            {
                await progressReporter.ReportAsync( new JobProgressDto
                {
                    Message = e.Message,
                    ProgressType = ProgressType.Error
                } );
            }
            EnablePolling();
        }

        public async Task StopAsync()
        {
            DisablePolling();

            if ( !isRegistered )
                return;

            try
            {
                await coreCiClient.Agents.DeregisterAsync();
            }
            catch ( Exception e )
            {
                Console.WriteLine( e.Message );
            }
        }

        private async Task OnRetryAsync( Exception exception, TimeSpan calculatedWaitDuration )
        {
            DisablePolling();
            await progressReporter.ReportAsync( new JobProgressDto
            {
                Message = $"{exception.Message} - retrying in {calculatedWaitDuration}",
                ProgressType = ProgressType.Error
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
