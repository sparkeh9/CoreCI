namespace CoreCI.Sdk
{
    public interface ICoreCI
    {
        IJobs Jobs { get; }
        IAuthenticator Authenticator { get; }
        IAgents Agents { get; }
    }
}
