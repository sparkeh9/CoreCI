namespace CoreCI.Kernel.Models
{
    using System.Collections.Generic;

    public class Pipeline
    {
        public IEnumerable<Step> Steps { get; set; }
        public string WorkspacePath { get; set; }
    }
}