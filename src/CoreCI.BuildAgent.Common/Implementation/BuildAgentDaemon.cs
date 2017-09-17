namespace CoreCI.BuildAgent.Common.Implementation
{
    using System;
    using System.Threading.Tasks;
    using CoreCI.Common.Models;
    using Polly;
    using Polly.Retry;
    using Sdk;

    public class BuildAgentDaemon : IBuildAgentDaemon
    {
        private readonly ICoreCI coreCiClient;
        private readonly RetryPolicy waitForeverPolicy;
        public event EventHandler<bool> PollStatusChanged;

        private bool isRegistered;

        public BuildAgentDaemon( ICoreCI coreCiClient )
        {
            this.coreCiClient = coreCiClient;

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

            if ( job == null )
            {
                EnablePolling();
                return;
            }

            Console.WriteLine( $"Reserved job {job.JobId}" );

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

        private TimeSpan Sleep( int attempt )
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
    }
}
