namespace CoreCI.Kernel.Infrastructure.Exceptions
{
    using System;

    public class LocalPathMissingException : Exception
    {
        public LocalPathMissingException() { }
        public LocalPathMissingException( string message ) : base( message ) { }
        public LocalPathMissingException( string message, Exception innerException ) : base( message, innerException ) { }
    }
}