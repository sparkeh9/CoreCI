namespace CoreCI.Kernel.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class Pipeline
    {
        public IEnumerable<Step> Steps { get; set; } = Enumerable.Empty<Step>();
        public string WorkspacePath { get; set; }
    }
}