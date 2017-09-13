namespace CoreCI.Sdk.Implementation.Http.Authentication
{
    using Flurl;
    using Flurl.Http;

    public class OAuthBearerAuthenticator : IAuthenticator
    {
        private readonly string token;

        public OAuthBearerAuthenticator( string token )
        {
            this.token = token;
        }

        public IFlurlClient Authenticate( Url url )
        {
            return url.WithOAuthBearerToken( token );
        }
    }
}
