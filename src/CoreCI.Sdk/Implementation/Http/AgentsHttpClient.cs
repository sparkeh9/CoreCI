﻿namespace CoreCI.Sdk.Implementation.Http
{
    using System.Threading.Tasks;
    using Common.Models;
    using Common.Models.Agents;
    using Flurl;
    using Flurl.Http;

    public class AgentsHttpClient : IAgents
    {
        private readonly CoreCIHttpClient coreCiHttpClient;

        public AgentsHttpClient( CoreCIHttpClient coreCiHttpClient )
        {
            this.coreCiHttpClient = coreCiHttpClient;
        }

        public async Task RegisterAsync( BuildEnvironment environment )
        {
            var response = await coreCiHttpClient.BaseApiUrl
                                                 .AppendPathSegment( "agents" )
                                                 .AppendPathSegment( "register" )
                                                 .Authenticate( coreCiHttpClient.Authenticator )
                                                 .PostJsonAsync( new RegisterAgentDto
                                                 {
                                                     BuildEnvironment = environment
                                                 } );

            response.EnsureSuccessStatusCode();
        }

        public async Task DeregisterAsync()
        {
            var response = await coreCiHttpClient.BaseApiUrl
                                                 .AppendPathSegment( "agents" )
                                                 .AppendPathSegment( "deregister" )
                                                 .Authenticate( coreCiHttpClient.Authenticator )
                                                 .PostJsonAsync( new { } );

            response.EnsureSuccessStatusCode();
        }
    }
}
