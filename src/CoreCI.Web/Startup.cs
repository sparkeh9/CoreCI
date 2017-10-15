namespace CoreCI.Web
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System.IO;
    using System.Reflection;
    using Infrastructure.Bootstrapping;
    using Infrastructure.Options;
    using Infrastructure.Routing;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.SpaServices.Webpack;
    using Microsoft.Extensions.PlatformAbstractions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        public IConfiguration Configuration { get; }

        /// <inheritdoc />
        public Startup( IConfiguration configuration ) => Configuration = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices( IServiceCollection services )
        {
            services.Configure<MongoDbOptions>( Configuration.GetSection( "MongoDb" ) );
            services.AddSwaggerGen( o =>
                                    {
                                        // resolve the IApiVersionDescriptionProvider service
                                        // note: that we have to build a temporary service provider here because one has not been created yet
                                        var provider = services.BuildServiceProvider()
                                                               .GetRequiredService<IApiVersionDescriptionProvider>();

                                        // add a swagger document for each discovered API version
                                        // note: you might choose to skip or document deprecated API versions differently
                                        foreach ( var description in provider.ApiVersionDescriptions )
                                        {
                                            o.SwaggerDoc( description.GroupName, CreateInfoForApiVersion( description ) );
                                        }

                                        // add a custom operation filter which sets default values
                                        o.OperationFilter<SwaggerDefaultValues>();
                                        o.DocumentFilter<LowercaseDocumentFilter>();

                                        // integrate xml comments
                                        o.IncludeXmlComments( XmlCommentsFilePath );
                                        o.DescribeAllEnumsAsStrings();
                                    } );
            services.AddMvcCore()
                    .AddVersionedApiExplorer( o => o.GroupNameFormat = "'v'VVV" );
            services.AddRouting( o =>
                                 {
                                     o.AppendTrailingSlash = false;
                                     o.LowercaseUrls = true;
                                     o.ConstraintMap.Add( "ObjectId", typeof( ObjectIdRouteConstraint ) );
                                 } );
            services.AddMvc()
                    .AddJsonOptions( o =>
                                     {
                                         o.SerializerSettings.Formatting = Formatting.Indented;
                                         o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                                         o.SerializerSettings.Converters.Add( new StringEnumConverter
                                         {
                                             CamelCaseText = true,
                                             AllowIntegerValues = true
                                         } );
                                     } );
            services.AddApiVersioning( o =>
                                       {
                                           o.DefaultApiVersion = new ApiVersion( 1, 0 );
                                           o.ReportApiVersions = true;
                                           o.AssumeDefaultVersionWhenUnspecified = true;
                                       } );

            return AutofacContainerBootstrapper.ConfigureServices( services );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider )
        {
            if ( env.IsDevelopment() )
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseWebpackDevMiddleware( new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ConfigFile = "webpack.dev.config.js"
                } );
            }
            else
            {
                app.UseExceptionHandler( "/Home/Error" );
            }

            app.UseStaticFiles();
            app.UseMvc( routes =>
                        {
                            routes.MapRoute( name : "default",
                                             template : "{controller=Home}/{action=Index}/{id?}" );

                            routes.MapSpaFallbackRoute("spa-fallback", new { controller = "Home", action = "Index" });
                        } );

            app.UseSwagger();
            app.UseSwaggerUI( options =>
                              {
                                  // build a swagger endpoint for each discovered API version
                                  foreach ( var description in provider.ApiVersionDescriptions )
                                  {
                                      options.SwaggerEndpoint( $"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant() );
                                  }
                              } );
        }

        private static Info CreateInfoForApiVersion( ApiVersionDescription description )
        {
            var info = new Info
            {
                Title = $"CoreCI API {description.ApiVersion}",
                Version = description.ApiVersion.ToString()
            };

            if ( description.IsDeprecated )
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }

        private static string XmlCommentsFilePath
        {
            get
            {
                string basePath = PlatformServices.Default.Application.ApplicationBasePath;
                string fileName = typeof( Startup ).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine( basePath, fileName );
            }
        }
    }
}
