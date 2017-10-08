namespace CoreCI.Common.Models.Agents
{
    using System.Collections.Generic;

    public class RegisterAgentDto
    {
        public List<BuildEnvironment> BuildEnvironments { get; set; }
    }
}
