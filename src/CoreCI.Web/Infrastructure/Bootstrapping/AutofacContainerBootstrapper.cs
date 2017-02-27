namespace CoreCI.Web.Infrastructure.Bootstrapping
{
    using System;
    using Autofac;
    using Microsoft.Extensions.DependencyInjection;
    using Autofac.Extensions.DependencyInjection;

    public static class AutofacContainerBootstrapper
    {
        public static IServiceProvider ConfigureServices( IServiceCollection services )
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyModules( AssemblyHelper.GetAssemblies( "ClientArea" ) );
            builder.Populate( services );
            var container = builder.Build();

            return container.Resolve<IServiceProvider>();
        }
    }
}