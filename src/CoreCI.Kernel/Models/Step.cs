namespace CoreCI.Kernel.Models
{
    using System.Collections.Generic;

    public class Step
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public IEnumerable<string> EnvironmentVariables { get; set; }
        public IEnumerable<string> Commands { get; set; }
    }
}