namespace CoreCI.Web
{
    using System;
    using FluentValidation.AspNetCore;
    using Infrastructure.Bootstrapping;
    using Infrastructure.Config;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup( IHostingEnvironment env )
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath( env.ContentRootPath )
                .AddJsonFile( "appsettings.json", false, true )
                .AddJsonFile( $"appsettings.{env.EnvironmentName}.json", true )
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices( IServiceCollection services )
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddRouting( RouteConfig.ConfigureOptions );
            services.AddAuthorization( AuthorisationConfig.ConfigureSecurityPolicies );
            services.AddMvc( AuthorisationConfig.WithAuthenticateByDefaultPolicy )
                    .AddFluentValidation( ValidationConfig.ConfigureValidation )
                    ;

            return AutofacContainerBootstrapper.ConfigureServices( services );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory )
        {
            loggerFactory.AddConsole( Configuration.GetSection( "Logging" ) );
            loggerFactory.AddDebug();

            if ( env.IsDevelopment() )
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler( "/Home/Error" );

            app.UseCookieAuthentication( AuthorisationConfig.ConfigureAuthorisation );
            app.UseStaticFiles();
            app.UseMvc( RouteConfig.ConfigureRoutes );
        }
    }
}