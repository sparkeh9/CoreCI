namespace CoreCI.Kernel.Infrastructure.Extensions
{
    using System.IO;
    using System.Text;

    public static class StringExtensions
    {
        public static bool IsNullOrEmpty( this string operand ) => string.IsNullOrEmpty( operand );
        public static bool IsNullOrWhiteSpace( this string operand ) => string.IsNullOrWhiteSpace( operand );
        public static string ToShortUuid( this string operand ) => operand.Substring( 0, 12 );

        public static string DumpToString( this Stream stream )
        {
            stream.Position = 0;
            using ( var reader = new StreamReader( stream, Encoding.UTF8 ) )
            {
                return reader.ReadToEnd();
            }
        }

        public static string ToTranslatedPath( this string operand )
        {
            return operand.Replace( '\\', '/' );
        }
    }
}