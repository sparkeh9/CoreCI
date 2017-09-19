namespace CoreCI.BuildAgent.Common.Implementation
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using CoreCI.Common.Models;
    using Exceptions;
    using Polly;
    using Polly.Retry;
    using Sdk;

    public class BuildAgentDaemon : IBuildAgentDaemon
    {
        private readonly ICoreCI coreCiClient;
        private readonly IVcsAppropriator vcsAppropriator;
        private readonly IBuildFileParser buildFileParser;
        private readonly IBuildProcessor buildProcessor;

        private readonly RetryPolicy waitForeverPolicy;
        public event EventHandler<bool> PollStatusChanged;

        private string lastMessage = string.Empty;
        private bool isRegistered;

        public BuildAgentDaemon( ICoreCI coreCiClient, IVcsAppropriator vcsAppropriator, IBuildFileParser buildFileParser, IBuildProcessor buildProcessor )
        {
            this.coreCiClient = coreCiClient;
            this.vcsAppropriator = vcsAppropriator;
            this.buildFileParser = buildFileParser;
            this.buildProcessor = buildProcessor;

            this.vcsAppropriator.OnProgress += ( sender, report ) =>
                                               {
                                                   if ( lastMessage == report )
                                                       return;

                                                   lastMessage = report;
                                                   Console.WriteLine( report );
                                               };

            this.buildProcessor.OnProgress += ( sender, report ) => { Console.WriteLine( report ); };

            waitForeverPolicy = Policy.Handle<Exception>()
                                      .WaitAndRetryForeverAsync( Sleep, onRetry : OnRetry );
        }

        public async Task InvokeAsync( BuildEnvironment environment )
        {
            DisablePolling();

            if ( !isRegistered )
                await waitForeverPolicy.ExecuteAsync( async () =>
                                                      {
                                                          Console.WriteLine( "Attempting to register build agent with coordinator" );
                                                          await coreCiClient.Agents.RegisterAsync( environment );
                                                          isRegistered = true;
                                                      } );

            Console.WriteLine( "Checking for available jobs" );
            var job = await coreCiClient.Jobs.ReserveFirstAvailableJobAsync( environment );

            if ( !job.HasValue )
            {
                EnablePolling();
                return;
            }

            try
            {
                string tempPath = GenerateTempPath();
                Console.WriteLine( $"Reserved job {job.Value.job.JobId}" );
                await vcsAppropriator.AcquireAsync( job.Value.job, tempPath );

                var buildFile = buildFileParser.ParseBuildFile( tempPath );
                buildProcessor.DoBuild( job.Value.job, buildFile, tempPath );
            }
            catch ( NoBuildFileFoundException e )
            {
                Console.WriteLine( e.Message );
            }
            catch ( Exception e )
            {
                Console.WriteLine( e.Message );
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

        private void OnRetry( Exception exception, TimeSpan calculatedWaitDuration )
        {
            DisablePolling();
            Console.WriteLine( $"{exception.Message} - retrying in {calculatedWaitDuration}" );
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

        private string GenerateTempPath()
        {
            return Path.GetFullPath( string.Join( Path.DirectorySeparatorChar.ToString(), new[]
            {
                Path.GetTempPath(),
                "coreci",
                Guid.NewGuid().ToString( "D" )
            } ) );
        }
    }
}
