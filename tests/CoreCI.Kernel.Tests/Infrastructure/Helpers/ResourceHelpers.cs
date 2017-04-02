namespace CoreCI.Kernel.Tests.Infrastructure.Helpers
{
    using System.IO;

    public static class ResourceHelpers
    {
        public static string GetTempFilePath() => Path.GetTempPath();
    }
}