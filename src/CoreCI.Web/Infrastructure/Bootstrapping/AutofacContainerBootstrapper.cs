namespace CoreCI.Web.Infrastructure.Bootstrapping
{
    using System;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Modules;
    using Options;

    public class AutofacContainerBootstrapper
    {
        public static IServiceProvider ConfigureServices( IServiceCollection services )
        {
            var mongoDbOptions = services.BuildServiceProvider()
                                         .GetService<IOptions<MongoDbOptions>>()
                                         .Value;

            var builder = new ContainerBuilder();
            builder.RegisterModule( new MongoDbModule( mongoDbOptions ) );
            builder.Populate( services );
            var container = builder.Build();

            return container.Resolve<IServiceProvider>();
        }
    }
}
