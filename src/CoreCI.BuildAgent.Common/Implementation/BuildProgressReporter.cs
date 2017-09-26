namespace CoreCI.BuildAgent.Common.Implementation
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using CoreCI.Common.Extensions;
    using Models;

    public class BuildProgressReporter : IBuildProgressReporter
    {
        private readonly (ConsoleColor foreground, ConsoleColor background) originalSettings;
        private static readonly BackgroundWorker backgroundWorker = new BackgroundWorker();

        public BuildProgressReporter()
        {
            originalSettings = (Console.ForegroundColor, Console.BackgroundColor);
            backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
        }

        public async Task ReportAsync( JobProgressDto progressItem )
        {
            backgroundWorker.RunWorkerAsync( progressItem );
            await Task.FromResult( 0 );
        }

        private void BackgroundWorkerOnDoWork( object sender, DoWorkEventArgs e )
        {
            if ( !( e.Argument is JobProgressDto dto ) )
            {
                return;
            }

            switch ( dto.ProgressType )
            {
                case ProgressType.Command:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case ProgressType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case ProgressType.Informational:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case ProgressType.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case ProgressType.Warning:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Console.WriteLine( dto.Message.RemoveControlCharacters() );
            Console.ForegroundColor = originalSettings.foreground;
            Console.BackgroundColor = originalSettings.background;
        }
    }
}
