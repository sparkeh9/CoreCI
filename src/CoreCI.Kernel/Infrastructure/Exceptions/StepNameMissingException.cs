namespace CoreCI.Kernel.Infrastructure.Exceptions
{
    using System;
    public class StepNameMissingException : Exception
    {
        public StepNameMissingException() { }
        public StepNameMissingException( string message ) : base( message ) { }
        public StepNameMissingException( string message, Exception innerException ) : base( message, innerException ) { }
    }
}