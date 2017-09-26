﻿namespace CoreCI.BuildAgent.Windows.Infrastructure.Modules
{
    using System.Configuration;
    using Autofac;
    using Common;
    using Common.Implementation;
    using Common.Models;
    using Common.Models.Docker;
    using Sdk;
    using Sdk.Implementation.Http;
    using Sdk.Implementation.Http.Authentication;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

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
                                  var config = ctx.Resolve<Configuration>();
                                  var dockerHost = config.AppSettings.Settings[ "dockerHost" ];
                                  var certificatePath = config.AppSettings.Settings[ "dockerCertificatePath" ];

                                  return new DockerConfiguration
                                  {
                                      RemoteEndpoint = dockerHost?.Value,
                                      CertificatePath = certificatePath?.Value
                                  };
                              } ).As<DockerConfiguration>();


            builder.Register( ctx =>
                              {
                                  var credentials = ctx.Resolve<ApiCredentials>();
                                  return new CoreCIHttpClient( credentials.Url ).WithAuthenticator( new OAuthBearerAuthenticator( credentials.Token ) );
                              } ).As<ICoreCI>();

            builder.Register( ctx =>
                                  new DeserializerBuilder()
                                      .WithNamingConvention( new CamelCaseNamingConvention() )
                                      .IgnoreUnmatchedProperties()
                                      .Build() )
                   .As<Deserializer>();

            builder.RegisterType<WindowsBuildAgentService>();
            builder.RegisterType<BuildAgentDaemon>().As<IBuildAgentDaemon>();

            builder.RegisterType<VcsAppropriator>().As<IVcsAppropriator>();
            builder.RegisterType<BuildFileParser>().As<IBuildFileParser>();
            builder.RegisterType<BuildProgressReporter>().As<IBuildProgressReporter>();
            builder.RegisterType<DockerBuildScriptGenerator>().As<IDockerBuildScriptGenerator>();
            builder.RegisterType<NativeBuildProcessor>().SingleInstance();
            builder.RegisterType<DockerBuildProcessor>().SingleInstance();
        }
    }
}
