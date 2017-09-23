namespace CoreCI.BuildAgent.Common.Models.Docker
{
    public class DockerConfiguration
    {
        public string RemoteEndpoint { get; set; }
        public string CertificatePath { get; set; }
        public byte[] PfxBytes { get; set; }
        public string PfxPassword { get; set; }
        public string PfxPath { get; set; }

        public DockerBasicAuth BasicAuth { get; set; }
    }
}
