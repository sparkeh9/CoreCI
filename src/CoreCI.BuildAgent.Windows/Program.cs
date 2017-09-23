namespace CoreCI.BuildAgent.Windows
{
    using System;
    using System.Configuration;
    using Autofac;
    using Infrastructure.Modules;
    using Topshelf;
    using Topshelf.Autofac;

    public class Program
    {
        private static IContainer container;

        private static void BuildContainer( Configuration config )
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<BuildAgentDaemonModule>();
            builder.RegisterInstance( config )
                   .As<Configuration>();
            container = builder.Build();
        }

        public static void Main( string[] args )
        {
            var config = ConfigurationManager.OpenExeConfiguration( ConfigurationUserLevel.None );
            BuildContainer( config );

            HostFactory.Run( configure =>
                             {
                                 configure.UseAutofacContainer( container );
                                 configure.AddCommandLineDefinition( "apiUrl", arg =>
                                                                               {
                                                                                   config.AppSettings.Settings.Remove( "apiUrl" );
                                                                                   config.AppSettings.Settings.Add( "apiUrl", arg );
                                                                                   config.Save( ConfigurationSaveMode.Modified );
                                                                               } );
                                 configure.AddCommandLineDefinition( "apiToken", arg =>
                                                                                 {
                                                                                     config.AppSettings.Settings.Remove( "apiToken" );
                                                                                     config.AppSettings.Settings.Add( "apiToken", arg );
                                                                                     config.Save( ConfigurationSaveMode.Modified );
                                                                                 } );
                                 configure.AddCommandLineDefinition( "dockerHost", arg =>
                                                                                   {
                                                                                       config.AppSettings.Settings.Remove( "dockerHost" );
                                                                                       config.AppSettings.Settings.Add( "dockerHost", arg );
                                                                                       config.Save( ConfigurationSaveMode.Modified );
                                                                                   } );
                                 configure.AddCommandLineDefinition( "dockerCertificatePath", arg =>
                                                                                              {
                                                                                                  config.AppSettings.Settings.Remove( "dockerCertificatePath" );
                                                                                                  config.AppSettings.Settings.Add( "dockerCertificatePath", arg );
                                                                                                  config.Save( ConfigurationSaveMode.Modified );
                                                                                              } );
                                 configure.ApplyCommandLine();
                                 configure.Service<WindowsBuildAgentService>( service =>
                                                                              {
                                                                                  service.ConstructUsingAutofacContainer();
                                                                                  service.WhenStarted( ( s, hostControl ) => s.Start( hostControl ) );
                                                                                  service.WhenStopped( ( s, hostControl ) => s.Stop( hostControl ) );
                                                                              } );

                                 configure.RunAsLocalSystem();
                                 configure.StartAutomatically();
                                 configure.EnableServiceRecovery( recovery =>
                                                                  {
                                                                      recovery.RestartService( 0 );
                                                                      recovery.RestartService( 1 );
                                                                      recovery.RestartService( 2 );

                                                                      recovery.OnCrashOnly();

                                                                      //number of days until the error count resets
                                                                      recovery.SetResetPeriod( 1 );
                                                                  } );

                                 configure.SetServiceName( "CoreCIBuildAgent" );
                                 configure.SetDisplayName( "CoreCIBuildAgent" );
                                 configure.SetDescription( "CoreCI Windows Build Agent" );

                                 configure.OnException( ex => { Console.WriteLine( ex.Message ); } );
                             } );
        }
    }
}
