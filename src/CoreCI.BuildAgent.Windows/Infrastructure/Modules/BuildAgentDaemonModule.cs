namespace CoreCI.BuildAgent.Windows.Infrastructure.Modules
{
    using System;
    using System.Configuration;
    using System.Security.Cryptography.X509Certificates;
    using Autofac;
    using Common.BuildAgentCore.BuildFile;
    using Common.BuildAgentCore.BuildProcessor;
    using Common.BuildAgentCore.Progress;
    using Common.BuildAgentCore.Vcs;
    using Common.Models;
    using Common.Models.Docker;
    using CoreCI.Common.Extensions;
    using Docker.DotNet;
    using Docker.DotNet.BasicAuth;
    using Docker.DotNet.X509;
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

            RegisterDocker( builder );

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

#if DEBUG
            builder.RegisterType<DebugOutputConsoleReporter>().As<IConsoleProgressReporter>();
#else
            builder.RegisterType<NullConsoleReporter>().As<IConsoleProgressReporter>();
#endif
        }

        private static void RegisterDocker( ContainerBuilder builder )
        {
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

            builder.Register<Credentials>( ctx =>
                                           {
                                               var dockerConfiguration = ctx.Resolve<DockerConfiguration>();
                                               var uri = new Uri( dockerConfiguration.RemoteEndpoint );

                                               if ( uri.Scheme == "npipe" || uri.Scheme == "unix" )
                                               {
                                                   return new AnonymousCredentials();
                                               }

                                               if ( !dockerConfiguration.PfxPath.IsNullOrWhiteSpace() )
                                               {
                                                   return dockerConfiguration.PfxPassword.IsNullOrEmpty()
                                                       ? new CertificateCredentials( new X509Certificate2( dockerConfiguration.PfxPath ) )
                                                       : new CertificateCredentials( new X509Certificate2( dockerConfiguration.PfxPath, dockerConfiguration.PfxPassword ) );
                                               }

                                               if ( dockerConfiguration.PfxBytes != null && dockerConfiguration.PfxBytes.Length > 0 )
                                               {
                                                   return dockerConfiguration.PfxPassword.IsNullOrEmpty()
                                                       ? new CertificateCredentials( new X509Certificate2( dockerConfiguration.PfxBytes ) )
                                                       : new CertificateCredentials( new X509Certificate2( dockerConfiguration.PfxBytes, dockerConfiguration.PfxPassword ) );
                                               }

                                               if ( dockerConfiguration.BasicAuth != null &&
                                                    !dockerConfiguration.BasicAuth.Username.IsNullOrWhiteSpace() &&
                                                    !dockerConfiguration.BasicAuth.Password.IsNullOrWhiteSpace() )
                                               {
                                                   return new BasicAuthCredentials( dockerConfiguration.BasicAuth.Username, dockerConfiguration.BasicAuth.Password, dockerConfiguration.BasicAuth.Tls );
                                               }

                                               return new AnonymousCredentials();
                                           } ).As<Credentials>();

            builder.Register( ctx =>
                              {
                                  var dockerConfiguration = ctx.Resolve<DockerConfiguration>();
                                  var credentials = ctx.Resolve<Credentials>();
                                  var config = new DockerClientConfiguration( new Uri( dockerConfiguration.RemoteEndpoint ), credentials );
                                  return config.CreateClient();
                              } ).As<DockerClient>();
        }
    }
}
