﻿namespace CoreCI.Kernel.Tests.Infrastructure.Extensions
{
    using System.IO;
    using System.Text;

    public static class StringExtensions
    {
        public static string ConvertToString( this Stream stream )
        {
            stream.Position = 0;
            using ( var reader = new StreamReader( stream, Encoding.UTF8 ) )
            {
                return reader.ReadToEnd();
            }
        }

        public static Stream ToStream( this string operand )
        {
            var byteArray = Encoding.UTF8.GetBytes( operand );
            return new MemoryStream( byteArray );
        }
    }
}