namespace CoreCI.Web.Infrastructure.Config
{
    using CoreCI.Web.Infrastructure;
    using FluentValidation.AspNetCore;

    public static class ValidationConfig
    {
        public static void ConfigureValidation( FluentValidationMvcConfiguration cfg )
        {
            cfg.ValidatorFactoryType = typeof( ServiceProviderValidatorFactory );
            cfg.RegisterValidatorsFromAssembly( AssemblyHelper.GetAssembly( "CoreCI.Web" ) );
        }
    }
}