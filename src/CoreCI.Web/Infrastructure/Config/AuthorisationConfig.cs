namespace CoreCI.Web.Infrastructure.Config
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Authorization;

    public static class AuthorisationConfig
    {
        public static CookieAuthenticationOptions ConfigureAuthorisation => new CookieAuthenticationOptions
        {
            AuthenticationScheme = "Cookie",
            LoginPath = new PathString( "/account/login" ),
            AccessDeniedPath = new PathString( "/account/login" ),
            AutomaticAuthenticate = true,
            AutomaticChallenge = true,
            CookieName = "CoreCI.Cookie"
        };

        public static void WithAuthenticateByDefaultPolicy( MvcOptions config )
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            config.Filters.Add( new AuthorizeFilter( policy ) );
        }

        public static void ConfigureSecurityPolicies( AuthorizationOptions config )
        {
            config.AddPolicy( "AdminOnlyPolicy", policy => policy.RequireRole( "Admin" ) );
//            config.AddPolicy( "ClientOnly", policy => policy.RequireRole( Role.Client.ToString() ) );
        }
    }
}