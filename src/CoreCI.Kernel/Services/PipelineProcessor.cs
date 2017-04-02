namespace CoreCI.Kernel.Services
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Microsoft.Extensions.Options;
    using Models;

    public class PipelineProcessor : IPipelineProcessor
    {
        private readonly PipelineProcessorOptions options;

        public PipelineProcessor( IOptions<PipelineProcessorOptions> configuration )
        {
            options = configuration.Value;
        }

        public async Task<PipelineProcessorResult> ProcessAsync( Pipeline pipeline, CancellationToken ctx = default( CancellationToken ) )
        {
            await Task.Delay( 0, ctx );
            ctx.ThrowIfCancellationRequested();

            var guid = Guid.NewGuid();
            var result = new PipelineProcessorResult
            {
                Identitifer = guid,
                WorkspacePath = GetWorkspacePath( guid )
            };

            Directory.CreateDirectory( result.WorkspacePath );

            return result;
        }

        public async Task CleanupWorkspaceAsync( Guid id )
        {
            await Task.Delay( 0 );
            string path = GetWorkspacePath( id );
            Directory.Delete( path, true );
        }

        private string GetWorkspacePath( Guid id )
        {
            return Path.Combine( options.Workspace, id.ToString( "N" ) );
        }
    }
}