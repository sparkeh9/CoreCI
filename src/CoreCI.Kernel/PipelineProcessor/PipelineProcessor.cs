namespace CoreCI.Kernel.PipelineProcessor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.BasicAuth;
    using Docker.DotNet.Models;
    using Docker.DotNet.X509;
    using Infrastructure.Extensions;
    using Microsoft.Extensions.Options;
    using Models;

    public class PipelineProcessor : IPipelineProcessor
    {
        private readonly PipelineProcessorOptions options;
        private readonly DockerClient dockerClient;

        public PipelineProcessor( IOptions<PipelineProcessorOptions> configuration )
        {
            options = configuration.Value;

            var credentials = GetDockerRemoteApiCredentials();
            var config = new DockerClientConfiguration( new Uri( "http://192.168.99.100:2376" ), credentials );
            dockerClient = config.CreateClient();
        }

        public async Task<PipelineProcessorResult> ProcessAsync( Pipeline pipeline, CancellationToken ctx = default( CancellationToken ) )
        {
            await Task.Delay( 0, ctx );
            ctx.ThrowIfCancellationRequested();

            var workspaceIdentifier = Guid.NewGuid();
            var result = new PipelineProcessorResult
            {
                Identitifer = workspaceIdentifier,
                WorkspacePath = GetWorkspacePath( workspaceIdentifier )
            };


            var steps = pipeline.Steps.ToList();
            await Task.WhenAll( steps.Select( step => dockerClient.Images.PullImageAsync( new ImagesPullParameters
            {
                All = false,
                Parent = step.Image,
                Tag = step.Image
            }, null ) ) );


            foreach ( var step in pipeline.Steps )
            {
                var res = await dockerClient.Containers
                                  .CreateContainerAsync( new CreateContainerParameters( new Config
                                                         {
                                                             Image = step.Image,
                                                             AttachStdout = true,
                                                             AttachStderr = true,
                                                             Entrypoint = step.Commands.ToList(),
                                                             Volumes = step.Volumes.ToDictionary( x => x.Key, x => new object() )
                                                         } )
                                                         {
                                                             HostConfig = new HostConfig
                                                             {
                                                                 Binds = step.Volumes
                                                                             .Select( x => $"{x.Key}:{x.Value}" )
                                                                             .ToList()
                                                             }
                                                         } );
            }


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


        private Credentials GetDockerRemoteApiCredentials()
        {
            if ( !options.PfxPath.IsNullOrWhiteSpace() )
                return new CertificateCredentials( new X509Certificate2( options.PfxPath ) );

            if ( options.BasicAuth != null &&
                 !options.BasicAuth.Username.IsNullOrWhiteSpace() &&
                 !options.BasicAuth.Password.IsNullOrWhiteSpace() )
                return new BasicAuthCredentials( options.BasicAuth.Username, options.BasicAuth.Password, options.BasicAuth.Tls );

            return new AnonymousCredentials();
        }
    }
}