namespace CoreCI.Kernel.Tests.Infrastructure.Helpers
{
    using System.Collections.Generic;
    using Models;

    public static class TestStepHelper
    {
        public static Image BasicAlpineImage()
        {
            return new Image
            {
                Parent = "alpine",
                Tag = "latest"
            };
        }

        public static List<string> DummyCommand()
        {
            return new List<string>
            {
                "ls"
            };
        }
    }
}