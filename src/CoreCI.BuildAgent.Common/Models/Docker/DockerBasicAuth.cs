namespace CoreCI.BuildAgent.Common.Models.Docker
{
    public class DockerBasicAuth
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Tls { get; set; } = true;
    }
}
