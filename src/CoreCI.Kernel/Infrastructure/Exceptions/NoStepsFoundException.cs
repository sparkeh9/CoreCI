namespace CoreCI.Kernel.Infrastructure.Exceptions
{
    using System;
    public class NoStepsFoundException : Exception
    {
        public NoStepsFoundException() { }
        public NoStepsFoundException( string message ) : base( message ) { }
        public NoStepsFoundException( string message, Exception innerException ) : base( message, innerException ) { }
    }
}