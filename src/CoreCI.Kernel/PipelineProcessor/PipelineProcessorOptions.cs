namespace CoreCI.Kernel.PipelineProcessor
{
    public class PipelineProcessorOptions
    {
        public string PfxPath { get; set; }
        public string RemoteEndpoint { get; set; }
        public string Workspace { get; set; }

        public BasicAuth BasicAuth { get; set; }
    }

    public class BasicAuth
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Tls { get; set; } = true;
    }
}