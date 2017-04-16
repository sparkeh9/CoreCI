namespace CoreCI.Kernel.Infrastructure.Extensions
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    public static class StreamExtensions
    {
        public static async Task WriteLineAsync( this Stream stream, string message )
        {
            var bytes = Encoding.UTF8.GetBytes( message + Environment.NewLine );
            await stream.WriteAsync( bytes, 0, bytes.Length );
        }
    }
}