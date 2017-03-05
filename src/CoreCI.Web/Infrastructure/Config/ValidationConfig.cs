namespace CoreCI.Web.Infrastructure.Config
{
    using FluentValidation.AspNetCore;

    public static class ValidationConfig
    {
        public static void ConfigureValidation( FluentValidationMvcConfiguration cfg )
        {
            cfg.RegisterValidatorsFromAssemblyContaining<Startup>();
        }
    }
}