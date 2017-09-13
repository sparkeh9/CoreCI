namespace CoreCI.Sdk
{
    using System.Threading.Tasks;
    using Common.Models;

    public interface IAgents
    {
        Task RegisterAsync( BuildEnvironment environment = BuildEnvironment.Any );
        Task DeregisterAsync();
    }
}
