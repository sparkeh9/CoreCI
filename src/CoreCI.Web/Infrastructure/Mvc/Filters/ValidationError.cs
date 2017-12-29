namespace CoreCI.Web.Infrastructure.Mvc.Filters
{
    public class ValidationError
    {
        public ValidationError( string propertyName, string errorMessage )
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }
        public string PropertyName { get; }
    }
}
