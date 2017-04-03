namespace CoreCI.Kernel.Models
{
    using System.Collections.Generic;

    public class Step
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public IDictionary<string, string> EnvironmentVariables { get; set; }
        public IDictionary<string, string> Volumes { get; set; }
        public IEnumerable<string> Commands { get; set; }
    }
}