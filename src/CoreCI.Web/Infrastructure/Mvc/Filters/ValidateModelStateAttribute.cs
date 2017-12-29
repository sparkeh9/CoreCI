namespace CoreCI.Web.Infrastructure.Mvc.Filters
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting( ActionExecutingContext context )
        {
            if ( context.ModelState.IsValid )
            {
                return;
            }

            var validationErrors =
                context.ModelState
                       .Where( keyValuePair => keyValuePair.Value.Errors.Any( modelError => !string.IsNullOrWhiteSpace( modelError.ErrorMessage ) ) )
                       .Select( keyValuePair => new ValidationError( keyValuePair.Key, keyValuePair.Value.Errors.Select( e => e.ErrorMessage ).First() ) )
                       .OrderBy( validationError => validationError.PropertyName )
                       .ToList();

            var logger = (ILogger<ValidateModelStateAttribute>) context.HttpContext.RequestServices.GetService( typeof( ILogger<ValidateModelStateAttribute> ) );
            logger.LogInformation( "ModelState is invalid. {@ValidationErrors}", validationErrors );

            var responseModel = validationErrors.Any()
                ? new ValidationFailedResponse( validationErrors )
                : null;

            context.Result = new BadRequestObjectResult( responseModel );
        }
    }
}
