namespace CoreCI.Kernel.Infrastructure.Extensions
{
    using System.IO;
    using System.Threading.Tasks;

    public static class StreamExtensions
    {
        public static async Task WriteLineAsync( this Stream stream, string message )
        {
            using ( var writer = new StreamWriter( stream ) )
            {
                await writer.WriteLineAsync( message );
            }
        }
    }
}