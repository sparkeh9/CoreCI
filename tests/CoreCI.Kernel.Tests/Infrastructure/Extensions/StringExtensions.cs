namespace CoreCI.Kernel.Tests.Infrastructure.Extensions
{
    using System.IO;
    using System.Text;

    public static class StringExtensions
    {
        public static Stream ToStream( this string operand )
        {
            var byteArray = Encoding.UTF8.GetBytes( operand );
            return new MemoryStream( byteArray );
        }
    }
}