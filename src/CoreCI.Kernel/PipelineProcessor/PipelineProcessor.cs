﻿namespace CoreCI.Kernel.PipelineProcessor
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
        private const int StreamBufferSize = 81920;
        private readonly Stream stdInStream;
        private readonly Stream stdOutStream;
        private readonly Stream errStream;
        private readonly PipelineProcessorOptions options;
        private readonly DockerClient dockerClient;
        private readonly List<string> startedContainers = new List<string>();

        public PipelineProcessor( IOptions<PipelineProcessorOptions> configuration, Stream stdInStream = null, Stream stdOutStream = null, Stream errStream = null )
        {
            this.stdInStream = stdInStream ?? new MemoryStream();
            this.stdOutStream = stdOutStream ?? new MemoryStream();
            this.errStream = errStream ?? new MemoryStream();

            options = configuration.Value;

            var credentials = GetDockerRemoteApiCredentials();
            var config = new DockerClientConfiguration( new Uri( "http://192.168.99.100:2376" ), credentials );
            dockerClient = config.CreateClient();
        }

        public async Task<PipelineProcessorResult> ProcessAsync( Pipeline pipeline, CancellationToken ctx = default( CancellationToken ) )
        {
            try
            {
                ctx.ThrowIfCancellationRequested();

                var workspaceIdentifier = Guid.NewGuid();
                var result = new PipelineProcessorResult
                {
                    Identitifer = workspaceIdentifier,
                    WorkspacePath = GetWorkspacePath( workspaceIdentifier )
                };

                Directory.CreateDirectory( result.WorkspacePath );

                var steps = pipeline.Steps.ToList();
                var containers = await GetStepContainers( steps );

                foreach ( var container in containers )
                {
                    ctx.ThrowIfCancellationRequested();
                    startedContainers.Add( container.ContainerId );

                    await dockerClient.Containers.StartContainerAsync( container.ContainerId, new ContainerStartParameters() );
                    var stdOutLogs = await dockerClient.Containers.GetContainerLogsAsync( container.ContainerId, new ContainerLogsParameters
                    {
                        ShowStdout = true
                    }, ctx );

                    var errorLogs = await dockerClient.Containers.GetContainerLogsAsync( container.ContainerId, new ContainerLogsParameters
                    {
                        ShowStderr = true
                    }, ctx );

                    await dockerClient.Containers.WaitContainerAsync( container.ContainerId, ctx );
                    await stdOutLogs.CopyToAsync( stdOutStream, StreamBufferSize, ctx );
                    await errorLogs.CopyToAsync( errStream, StreamBufferSize, ctx );
                }

                return result;
            }
            catch ( Exception e )
            {
                await errStream.WriteLineAsync( e.Message );
                throw;
            }
            finally
            {
                await stdOutStream.WriteLineAsync( "Cleaning up" );
                foreach ( string containerId in startedContainers )
                {
                    await dockerClient.Containers.StopContainerAsync( containerId, new ContainerStopParameters(), CancellationToken.None );
                    await dockerClient.Containers.RemoveContainerAsync( containerId, new ContainerRemoveParameters() );
                }
            }
        }

        private async Task<IEnumerable<StepContainer>> GetStepContainers( IReadOnlyCollection<Step> steps )
        {
            await stdOutStream.WriteLineAsync( "Generating steps" );
            await PullAllRequiredImagesAsync( steps );

            var containerResponses = new List<StepContainer>();

            foreach ( var step in steps )
            {
                await stdOutStream.WriteLineAsync( $"- {step.Name} ({step.Image})" );
                var containerResponse = await dockerClient.Containers
                                                          .CreateContainerAsync( new CreateContainerParameters( new Config
                                                                                 {
                                                                                     Image = step.Image.ToString(),
                                                                                     AttachStdout = true,
                                                                                     AttachStderr = true,
                                                                                     Entrypoint = step?.Commands.ToList(),
//                                                                                     Volumes = step != null
//                                                                                         ? step.Volumes?.ToDictionary( x => x.Key, x => new object() )
//                                                                                         : new Dictionary<string, object>()
                                                                                 } )
                                                                                 {
                                                                                     HostConfig = new HostConfig
                                                                                     {
//                                                                                         Binds = step != null
//                                                                                             ? step?.Volumes
//                                                                                                   .Select( x => $"{x.Key}:{x.Value}" )
//                                                                                                   .ToList()
//                                                                                             : new List<string>()
                                                                                     }
                                                                                 } );

                containerResponses.Add( new StepContainer
                {
                    Step = step,
                    ContainerId = containerResponse.ID.ToShortUuid()
                } );
            }
            return containerResponses;
        }

        private async Task PullAllRequiredImagesAsync( IEnumerable<Step> steps )
        {
            await Task.WhenAll( steps.Select( step => dockerClient.Images.PullImageAsync( new ImagesPullParameters
            {
                All = false,
                Parent = step.Image.Parent,
                Tag = step.Image.Tag
            }, null ) ) );
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

        private class StepContainer
        {
            public Step Step { get; set; }
            public string ContainerId { get; set; }
        }
    }
}