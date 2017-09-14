namespace CoreCI.BuildAgent.Common.Implementation
{
    using System;
    using System.Threading.Tasks;
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

        public async Task InvokeAsync()
        {
            PollStatusChanged?.Invoke( this, false );

            if ( !isRegistered )
                await waitForeverPolicy.ExecuteAsync( async () =>
                                                      {
                                                          Console.WriteLine( "Attempting to register build agent with coordinator" );
                                                          await coreCiClient.Agents.RegisterAsync();
                                                          isRegistered = true;
                                                      } );

            PollStatusChanged?.Invoke( this, true );
        }

        public async Task StopAsync()
        {
            PollStatusChanged?.Invoke( this, false );

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
            PollStatusChanged?.Invoke( this, false );
            Console.WriteLine( $"{exception.Message} - retrying in {calculatedWaitDuration}" );
        }

        private TimeSpan Sleep( int attempt )
        {
            return TimeSpan.FromSeconds( 30 );
        }
    }
}
