namespace CoreCI.Kernel
{
    using System.Collections.Generic;

    public class BuildStep
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public IEnumerable<string> EnvironmentVariables { get; set; }
        public IEnumerable<string> Commands { get; set; }
    }
}