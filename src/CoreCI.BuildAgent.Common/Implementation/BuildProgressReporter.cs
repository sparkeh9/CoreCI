namespace CoreCI.BuildAgent.Common.Implementation
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using CoreCI.Common.Models.Jobs;
    using Sdk;

    public class BuildProgressReporter : IBuildProgressReporter
    {
        private readonly (ConsoleColor foreground, ConsoleColor background) originalSettings;
        private static readonly BackgroundWorker backgroundWorker = new BackgroundWorker();

        private readonly ICoreCI coreCiClient;

        public BuildProgressReporter( ICoreCI coreCiClient )
        {
            this.coreCiClient = coreCiClient;
            originalSettings = (Console.ForegroundColor, Console.BackgroundColor);


            backgroundWorker.DoWork += BackgroundWorkerOnDoWorkAsync;
        }

        public async Task ReportAsync( JobProgressDto progressItem )
        {
            backgroundWorker.RunWorkerAsync( progressItem );
            await Task.FromResult( 0 );
        }

        private async void BackgroundWorkerOnDoWorkAsync( object sender, DoWorkEventArgs args )
        {
            if ( !( args.Argument is JobProgressDto dto ) )
            {
                return;
            }

            await coreCiClient.Jobs.ReportAsync( dto );

            switch ( dto.JobProgressType )
            {
                case JobProgressType.Command:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case JobProgressType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case JobProgressType.Informational:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case JobProgressType.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case JobProgressType.Warning:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Console.WriteLine( dto.Message );
            Console.ForegroundColor = originalSettings.foreground;
            Console.BackgroundColor = originalSettings.background;
        }
    }
}
