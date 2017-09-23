namespace CoreCI.Common.Extensions
{
    using System.Linq;

    public static class StringExtensions
    {
        public static bool IsNullOrEmpty( this string operand ) => string.IsNullOrEmpty( operand );
        public static bool IsNullOrWhiteSpace( this string operand ) => string.IsNullOrWhiteSpace( operand );
        public static string RemoveControlCharacters( this string operand ) => new string( operand.Where( x => !char.IsControl( x ) ).ToArray() );
    }
}
