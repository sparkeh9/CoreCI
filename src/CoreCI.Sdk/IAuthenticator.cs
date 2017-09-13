namespace CoreCI.Sdk
{
    using Flurl;
    using Flurl.Http;

    public interface IAuthenticator
    {
        IFlurlClient Authenticate( Url url );
    }
}
