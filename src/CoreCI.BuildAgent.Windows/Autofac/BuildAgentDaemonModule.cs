namespace CoreCI.BuildAgent.Windows.Autofac
{
    using System.Configuration;
    using global::Autofac;
    using Common;
    using Common.Implementation;
    using Common.Models;
    using Sdk;
    using Sdk.Implementation.Http;
    using Sdk.Implementation.Http.Authentication;

    public class BuildAgentDaemonModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.Register( ctx =>
                              {
                                  var config = ctx.Resolve<Configuration>();
                                  var apiUrl = config.AppSettings.Settings[ "apiUrl" ];
                                  var apiToken = config.AppSettings.Settings[ "apiToken" ];

                                  return new ApiCredentials
                                  {
                                      Url = apiUrl.Value,
                                      Token = apiToken.Value
                                  };
                              } ).As<ApiCredentials>();


            builder.Register( ctx =>
                              {
                                  var credentials = ctx.Resolve<ApiCredentials>();
                                  return new CoreCIHttpClient( credentials.Url ).WithAuthenticator( new OAuthBearerAuthenticator( credentials.Token ) );
                              } ).As<ICoreCI>();

            builder.RegisterType<BuildAgentDaemon>().As<IBuildAgentDaemon>();
            builder.RegisterType<WindowsBuildAgentService>();
        }
    }
}
