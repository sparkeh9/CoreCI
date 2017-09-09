namespace CoreCI.Web
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System.IO;
    using System.Reflection;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.PlatformAbstractions;
    using Newtonsoft.Json.Converters;
    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        public IConfiguration Configuration { get; }

        /// <inheritdoc />
        public Startup( IConfiguration configuration ) => Configuration = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices( IServiceCollection services )
        {
            services.AddSwaggerGen( options =>
                                    {
                                        // resolve the IApiVersionDescriptionProvider service
                                        // note: that we have to build a temporary service provider here because one has not been created yet
                                        var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                                        // add a swagger document for each discovered API version
                                        // note: you might choose to skip or document deprecated API versions differently
                                        foreach ( var description in provider.ApiVersionDescriptions )
                                        {
                                            options.SwaggerDoc( description.GroupName, CreateInfoForApiVersion( description ) );
                                        }

                                        // add a custom operation filter which sets default values
                                        options.OperationFilter<SwaggerDefaultValues>();

                                        // integrate xml comments
                                        options.IncludeXmlComments( XmlCommentsFilePath );
                                        options.DescribeAllEnumsAsStrings();
                                    } );
            services.AddMvcCore()
                    .AddVersionedApiExplorer( o => o.GroupNameFormat = "'v'VVV" );

            services.AddMvc()
                    .AddJsonOptions( options =>
                                     {
                                         options.SerializerSettings.Converters.Add( new StringEnumConverter
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider )
        {
            if ( env.IsDevelopment() )
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
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
