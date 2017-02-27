namespace CoreCI.Web.Infrastructure.Config
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;

    public static class RouteConfig
    {
        public static void ConfigureOptions( RouteOptions routeOptions )
        {
            routeOptions.LowercaseUrls = true;
        }

        public static void ConfigureRoutes( IRouteBuilder routes )
        {
            routes.MapRoute( name: "areaRoute",
                             template: "{area:exists}/{controller}/{action}/{id?}",
                             defaults: new { controller = "Home", action = "Index" } );

            routes.MapRoute( name: "ErrorRouting",
                             template: "error/{statusCode}",
                             defaults: new { controller = "Error", action = "Index" } );

            routes.MapRoute( name: "default",
                             template: "{controller=Home}/{action=Index}/{id?}" );
        }
    }
}