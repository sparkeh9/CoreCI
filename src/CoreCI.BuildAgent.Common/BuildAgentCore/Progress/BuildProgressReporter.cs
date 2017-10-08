namespace CoreCI.BuildAgent.Common.BuildAgentCore.Progress
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using CoreCI.Common.Models.Jobs;
    using Sdk;

    public class BuildProgressReporter : IBuildProgressReporter
    {
        private readonly ICoreCI coreCiClient;
        private readonly IConsoleProgressReporter consoleReporter;
        private string buildAgentToken;
        private static readonly BackgroundWorker backgroundWorker = new BackgroundWorker();

        public BuildProgressReporter( ICoreCI coreCiClient, IConsoleProgressReporter consoleReporter )
        {
            this.coreCiClient = coreCiClient;
            this.consoleReporter = consoleReporter;
            backgroundWorker.DoWork += BackgroundWorkerOnDoWorkAsync;
        }

        public async Task ReportAsync( JobProgressDto progressItem )
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter( TimeSpan.FromSeconds( 10 ) );
            while ( backgroundWorker.IsBusy )
            {
                cts.Token.ThrowIfCancellationRequested();
                Thread.Sleep( 100 );
            }
            backgroundWorker.RunWorkerAsync( progressItem );
            await Task.FromResult( 0 );
        }

        public void UseBuildAgentToken( string token )
        {
            buildAgentToken = token;
        }

        private async void BackgroundWorkerOnDoWorkAsync( object sender, DoWorkEventArgs args )
        {
            if ( !( args.Argument is JobProgressDto dto ) )
            {
                return;
            }

            dto.BuildAgentToken = buildAgentToken;
            await coreCiClient.Jobs.ReportAsync( dto );
            consoleReporter?.Report( dto );
        }
    }
}
