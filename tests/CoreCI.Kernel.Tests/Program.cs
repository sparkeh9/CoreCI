namespace CoreCI.Kernel.Tests
{
    using System;
    using Infrastructure.Logger;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;

    public static class Program
    {
        public static ILoggerFactory LoggerFactory;

        public static void Main( string[] args )
        {
            Initialise();
        }

        public static void Initialise( ITestOutputHelper outputHelper = null )
        {
            LoggerFactory = new LoggerFactory()
                .AddXunitConsole( outputHelper )
                .AddDebug( LogLevel.Debug );

            var builder = new ConfigurationBuilder()
                .SetBasePath( AppContext.BaseDirectory )
                .AddJsonFile( "appsettings.json", true, true );

            var configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddOptions();

            var serviceProvider = services.BuildServiceProvider();
        }
    }
}