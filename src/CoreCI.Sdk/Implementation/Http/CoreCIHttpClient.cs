namespace CoreCI.Sdk.Implementation.Http
{
    public class CoreCIHttpClient : ICoreCI
    {
        public string BaseUrl { get; }
        public string BaseApiUrl => $"{BaseUrl}/api/v1.0";

        public CoreCIHttpClient( string baseUrl )
        {
            BaseUrl = baseUrl;
        }

        public IAuthenticator Authenticator { get; protected set; }
        public IAgents Agents => new AgentsHttpClient( this );
        public IJobs Jobs => new JobsHttpClient( this );

        public ICoreCI WithAuthenticator( IAuthenticator authenticator )
        {
            Authenticator = authenticator;
            return this;
        }
    }
}
