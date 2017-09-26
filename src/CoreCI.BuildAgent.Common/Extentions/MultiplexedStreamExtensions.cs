namespace CoreCI.BuildAgent.Common.Extentions
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;

    public static class MultiplexedStreamExtensions
    {
        public static async Task<string> ReadLineAsync( this MultiplexedStream operand )
        {
            var data = new List<byte>();
            var buffer = new byte[ 1 ];
            string line = null;

            while ( !( await operand.ReadOutputAsync( buffer, 0, buffer.Length, CancellationToken.None ) ).EOF )
            {
                data.Add( buffer[ 0 ] );
                if ( (char) buffer[ 0 ] != '\n' )
                {
                    continue;
                }
                line = Encoding.UTF8.GetString( data.ToArray() ).Trim( '\r', '\n' );
                break;
            }

            return line;
        }
    }
}
