namespace CoreCI.Sdk
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Models;

    public interface IAgents
    {
        Task RegisterAsync( List<BuildEnvironment> environment );
        Task DeregisterAsync();
    }
}
