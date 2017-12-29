namespace CoreCI.Web.Infrastructure.Mvc.Filters
{
    using System.Collections.Generic;

    public class ValidationFailedResponse : ErrorResponse
    {
        public ValidationFailedResponse( IReadOnlyCollection<ValidationError> validationErrors )
            : base( "ValidationFailed", "The request contains one or more validation errors." )
        {
            ValidationErrors = validationErrors;
        }

        public IReadOnlyCollection<ValidationError> ValidationErrors { get; }
    }
}
