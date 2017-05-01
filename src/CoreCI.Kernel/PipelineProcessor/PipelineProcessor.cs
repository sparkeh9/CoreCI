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
    using Infrastructure.Exceptions;
    using Infrastructure.Extensions;
    using Microsoft.Extensions.Logging;
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
        private Guid workspaceIdentifier;

        public ILogger Logger { get; set; }

        public PipelineProcessor( IOptions<PipelineProcessorOptions> configuration, Stream stdInStream = null, Stream stdOutStream = null, Stream errStream = null )
        {
            this.stdInStream = stdInStream ?? new MemoryStream();
            this.stdOutStream = stdOutStream ?? new MemoryStream();
            this.errStream = errStream ?? new MemoryStream();

            options = configuration.Value;

            var credentials = GetDockerRemoteApiCredentials();
            var config = new DockerClientConfiguration( new Uri( options.RemoteEndpoint ), credentials );
            dockerClient = config.CreateClient();
        }

        public async Task<PipelineProcessorResult> ProcessAsync( Pipeline pipeline, CancellationToken ctx = default(CancellationToken) )
        {
            try
            {
                Logger?.LogInformation( "Processing pipeline" );
                ctx.ThrowIfCancellationRequested();

                if ( pipeline.Steps == null || !pipeline.Steps.Any() )
                    throw new NoStepsFoundException();

                workspaceIdentifier = Guid.NewGuid();

                var result = new PipelineProcessorResult
                {
                    Identitifer = workspaceIdentifier,
                    WorkspacePath = !pipeline.WorkspacePath.IsNullOrWhiteSpace() && pipeline.WorkspacePath != "/workspace"
                        ? pipeline.WorkspacePath
                        : GetWorkspacePath( workspaceIdentifier )
                };


                Logger?.LogDebug( $"Creating workspace directory {result.WorkspacePath}" );
                Directory.CreateDirectory( result.WorkspacePath );

                var steps = pipeline.Steps?.ToList();

                await GenerateStepScripts( result.Identitifer, steps, result.WorkspacePath );
                var containers = await GetStepContainersAsync( steps, result.WorkspacePath.ToTranslatedPath() );

                foreach ( var container in containers )
                {
                    await WriteToProcessLog( $"Executing step - {container.Step.Name}" );
                    ctx.ThrowIfCancellationRequested();
                    startedContainers.Add( container.ContainerId );

                    await dockerClient.Containers.StartContainerAsync( container.ContainerId, new ContainerStartParameters() );

                    await dockerClient.Containers.WaitContainerAsync( container.ContainerId, ctx );
                    var stdOutLogs = await dockerClient.Containers.GetContainerLogsAsync( container.ContainerId,
                                                                                          new ContainerLogsParameters
                                                                                          {
                                                                                              ShowStdout = true
                                                                                          }, ctx );

                    var errorLogs = await dockerClient.Containers.GetContainerLogsAsync( container.ContainerId,
                                                                                         new ContainerLogsParameters
                                                                                         {
                                                                                             ShowStderr = true
                                                                                         }, ctx );


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
                await WriteToProcessLog( "Cleaning up" );
                foreach ( string containerId in startedContainers )
                {
                    await dockerClient.Containers.StopContainerAsync( containerId, new ContainerStopParameters(), CancellationToken.None );
                    await dockerClient.Containers.RemoveContainerAsync( containerId, new ContainerRemoveParameters() );
                }
            }
        }

        private async Task GenerateStepScripts( Guid? resultIdentitifer, List<Step> steps, string resultWorkspacePath )
        {
            Logger?.LogDebug( $"Generating step scripts" );
            foreach ( var step in steps )
            {
                var commands = step.Commands;

                if ( commands == null || !commands.Any() )
                    continue;

                using ( TextWriter tw = File.CreateText( $"{resultWorkspacePath}/{step.Name}_{resultIdentitifer}.sh" ) )
                {
                    tw.NewLine = "\n";
                    await tw.WriteLineAsync( "#!/bin/sh" );
                    await tw.WriteLineAsync( "cd /workspace" );

                    foreach ( var command in commands )
                    {
                        await tw.WriteLineAsync( command );
                    }
                }
            }
        }


        /// <summary>
        /// Generates a list of container IDs representing each step.
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="workspacePath"></param>
        /// <returns></returns>
        private async Task<IEnumerable<StepContainer>> GetStepContainersAsync( IReadOnlyCollection<Step> steps, string workspacePath )
        {
            Logger?.LogDebug( $"Creating step containers" );
            if ( steps == null || !steps.Any() )
                return Enumerable.Empty<StepContainer>();

            await PullAllRequiredImagesAsync( steps );

            var containerResponses = new List<StepContainer>();

            foreach ( var step in steps )
            {
                if ( step.Name.IsNullOrWhiteSpace() )
                    throw new StepNameMissingException( $"With image: {step.Image}" );

                var containerResponse = await dockerClient.Containers
                                                          .CreateContainerAsync( new CreateContainerParameters( new
                                                                                                                    Config
                                                                                                                    {
                                                                                                                        Image = step.Image.ToString(),
                                                                                                                        AttachStdout = true,
                                                                                                                        AttachStderr = true,
                                                                                                                        Entrypoint = step.Commands != null
                                                                                                                            ? new List<string>
                                                                                                                            {
                                                                                                                                "sh",
                                                                                                                                $"/workspace/{step.Name}_{workspaceIdentifier}.sh" //                                                                                                                                "ls","-al","workspace"
//                                                                                                                                $"chmod +x /workspace/{step.Name}_{workspaceIdentifier}.sh",
//                                                                                                                                scriptPath
                                                                                                                            }
                                                                                                                            : null,
                                                                                                                        Env = step.EnvironmentVariables?.Select( x => $"{x.Key}={x.Value}" ).ToList(),
                                                                                                                        Volumes = step.ExtractVolumes( workspacePath ),
                                                                                                                    } )
                                                          {
                                                              HostConfig = new HostConfig
                                                              {
                                                                  Binds = step.ExtractBindings( workspacePath ).ToList()
                                                              }
                                                          } );

                containerResponses.Add( new StepContainer
                {
                    Step = step,
                    ContainerId = containerResponse.ID //.ToShortUuid()
                } );
            }
            return containerResponses;
        }

        private async Task PullAllRequiredImagesAsync( IEnumerable<Step> steps )
        {
            Logger?.LogDebug( $"Pulling required images" );

            foreach ( var step in steps )
            {
                var foundImage = await dockerClient.Images.ListImagesAsync( new ImagesListParameters
                {
                    All = false,
                    MatchName = step.Image.ToString()
                } );

                if ( foundImage.Any() )
                    continue;

                await WriteToProcessLog( $"Image {step.Image} not found locally. Pulling" );

                var pullImageStream = await dockerClient.Images.CreateImageAsync( new ImagesCreateParameters
                {
                    Parent = step.Image.Parent,
                    Tag = step.Image.Tag,
                }, null );

                await pullImageStream.CopyToAsync( stdOutStream );
            }
        }

        public async Task CleanupWorkspaceAsync( Guid id )
        {
            Logger?.LogDebug( $"Cleaning up workspace" );
            await Task.Delay( 0 );
            string path = GetWorkspacePath( id );

            if ( Directory.Exists( path ) )
                Directory.Delete( path, true );
        }

        private string GetWorkspacePath( Guid id )
        {
            return Path.Combine( options.Workspace, id.ToString( "N" ) );
        }

        private Credentials GetDockerRemoteApiCredentials()
        {
            Logger?.LogDebug( $"Determining docker remote API credentials" );
            var uri = new Uri( options.RemoteEndpoint );

            if ( uri.Scheme == "npipe" )
                return new AnonymousCredentials();

            if ( !options.PfxPath.IsNullOrWhiteSpace() )
            {
                if ( options.PfxPassword.IsNullOrEmpty() )
                {
                    return new CertificateCredentials( new X509Certificate2( options.PfxPath ) );
                }
                return new CertificateCredentials( new X509Certificate2( options.PfxPath, options.PfxPassword ) );
            }

            if ( options.PfxBytes != null && options.PfxBytes.Length > 0 )
            {
                if ( options.PfxPassword.IsNullOrEmpty() )
                {
                    return new CertificateCredentials( new X509Certificate2( options.PfxBytes ) );
                }
                return new CertificateCredentials( new X509Certificate2( options.PfxBytes, options.PfxPassword ) );
            }

            if ( options.BasicAuth != null &&
                 !options.BasicAuth.Username.IsNullOrWhiteSpace() &&
                 !options.BasicAuth.Password.IsNullOrWhiteSpace() )
                return new BasicAuthCredentials( options.BasicAuth.Username, options.BasicAuth.Password, options.BasicAuth.Tls );

            return new AnonymousCredentials();
        }

        private async Task WriteToProcessLog( string message )
        {
            Logger?.LogDebug( message );
            await stdOutStream.WriteLineAsync( message );
        }

        private class StepContainer
        {
            public Step Step { get; set; }
            public string ContainerId { get; set; }
        }
    }
}