namespace CoreCI.BuildAgent.Common
{
    using System.Threading.Tasks;

    public interface IBuildAgentDaemon
    {
        Task<bool> StartAsync();
        Task<bool> StopAsync();
    }
}
