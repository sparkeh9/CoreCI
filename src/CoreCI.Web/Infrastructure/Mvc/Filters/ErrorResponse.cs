namespace CoreCI.Web.Infrastructure.Mvc.Filters
{
    public class ErrorResponse
    {
        public ErrorResponse()
            : this( "UnexpectedError", "An unexpected error has occurred." ) { }

        public ErrorResponse( string errorCode, string errorMessage )
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public string ErrorCode { get; }
        public string ErrorMessage { get; }
    }
}
