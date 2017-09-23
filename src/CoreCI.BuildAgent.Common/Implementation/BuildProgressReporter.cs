namespace CoreCI.BuildAgent.Common.Implementation
{
    using System;
    using System.Threading.Tasks;
    using Models;

    public class BuildProgressReporter : IBuildProgressReporter
    {
        private readonly (ConsoleColor foreground, ConsoleColor background) originalSettings;

        public BuildProgressReporter()
        {
            originalSettings = (Console.ForegroundColor, Console.BackgroundColor);
        }

        public async Task ReportAsync( JobProgressDto progressItem )
        {
            await ReportAsync( null, progressItem );
        }

        public async Task ReportAsync( string buildAgentToken, JobProgressDto progressItem )
        {
            await Task.FromResult( 0 );
            switch ( progressItem.ProgressType )
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
            }

            Console.WriteLine( progressItem.Message );
            Console.ForegroundColor = originalSettings.foreground;
            Console.BackgroundColor = originalSettings.background;
        }
    }
}
