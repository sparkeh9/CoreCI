namespace CoreCI.BuildAgent.Common.Implementation
{
    using System.Threading.Tasks;
    using Sdk;

    public class BuildAgentDaemon : IBuildAgentDaemon
    {
        private readonly ICoreCI coreCiClient;

        public BuildAgentDaemon( ICoreCI coreCiClient )
        {
            this.coreCiClient = coreCiClient;
        }

        public async Task<bool> StartAsync()
        {
            await coreCiClient.Agents.RegisterAsync();
            return true;
        }

        public async Task<bool> StopAsync()
        {
            await coreCiClient.Agents.DeregisterAsync();
            return true;
        }
    }
}
