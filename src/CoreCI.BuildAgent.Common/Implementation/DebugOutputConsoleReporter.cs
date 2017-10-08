namespace CoreCI.BuildAgent.Common.Implementation
{
    using System;
    using CoreCI.Common.Models.Jobs;

    public class DebugOutputConsoleReporter : IConsoleProgressReporter
    {
        private readonly (ConsoleColor foreground, ConsoleColor background) originalSettings;

        public DebugOutputConsoleReporter()
        {
            originalSettings = (Console.ForegroundColor, Console.BackgroundColor);
        }

        public void Report( JobProgressDto dto )
        {
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
