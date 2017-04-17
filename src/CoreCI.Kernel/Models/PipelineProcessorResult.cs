namespace CoreCI.Kernel.Models
{
    using System;

    public class PipelineProcessorResult
    {
        public Guid? Identitifer { get; set; }
        public string WorkspacePath { get; set; }
    }
}