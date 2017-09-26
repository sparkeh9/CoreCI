namespace CoreCI.BuildAgent.Common.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using CoreCI.Common.Extensions;
    using CoreCI.Common.Models;
    using CoreCI.Common.Models.Jobs;
    using Docker.DotNet;
    using Docker.DotNet.BasicAuth;
    using Docker.DotNet.Models;
    using Docker.DotNet.X509;
    using Konsole;
    using Models;
    using Models.BuildFile;
    using Models.Docker;

    public class DockerBuildProcessor : IBuildProcessor
    {
        private readonly DockerClient dockerClient;
        private readonly DockerConfiguration dockerConfiguration;
        private readonly Dictionary<string, ProgressBar> progressBars = new Dictionary<string, ProgressBar>();

        public DockerBuildProcessor( DockerConfiguration dockerConfiguration )
        {
            this.dockerConfiguration = dockerConfiguration;

            var credentials = GetDockerRemoteApiCredentials();
            var config = new DockerClientConfiguration( new Uri( dockerConfiguration.RemoteEndpoint ), credentials );
            dockerClient = config.CreateClient();
        }

        public event EventHandler<JobProgressDto> OnProgress;

        public async Task DoBuildAsync( JobDto job, BuildFile buildFile, string path )
        {
            job.Environment = BuildEnvironment.Windows;
            await GenerateStepScripts( job, buildFile, path );

            var foundImage = await dockerClient.Images.ListImagesAsync( new ImagesListParameters
            {
                All = false,
                MatchName = buildFile.DockerImage.ToString()
            } );

            if ( !foundImage.Any() )
            {
                OnProgress?.Invoke( this, new JobProgressDto
                {
                    ProgressType = ProgressType.Command,
                    Message = $"Pulling image {buildFile.DockerImage}"
                } );

                await dockerClient.Images
                                  .CreateImageAsync( new ImagesCreateParameters
                                                     {
                                                         FromImage = buildFile.DockerImage.Parent,
                                                         Tag = buildFile.DockerImage.Tag
                                                     },
                                                     null,
                                                     new Progress<JSONMessage>( ReportDockerPullProgress ) );

                progressBars.Clear();
            }

            OnProgress?.Invoke( this, new JobProgressDto
            {
                ProgressType = ProgressType.Command,
                Message = $"Creating container from image {buildFile.DockerImage}"
            } );

            string filename = job.Environment == BuildEnvironment.Windows
                ? @"c:\workspace\coreci_build_steps.ps1"
                : "/workspace/coreci_build_steps.sh";

            var container = await dockerClient.Containers
                                              .CreateContainerAsync( new CreateContainerParameters( new Config
                                                                     {
                                                                         Image = buildFile.DockerImage.ToString(),

                                                                         AttachStdout = true,
                                                                         AttachStderr = true,
                                                                         Entrypoint = new List<string>
                                                                         {
                                                                             job.Environment == BuildEnvironment.Windows
                                                                                 ? "powershell"
                                                                                 : "sh",
                                                                             filename
                                                                         },
//                                  Env = step.EnvironmentVariables?.Select( x => $"{x.Key}={x.Value}" ).ToList(),
                                                                         Volumes = new Dictionary<string, EmptyStruct>
                                                                         {
                                                                             {
                                                                                 job.Environment == BuildEnvironment.Windows
                                                                                     ? @"c:\workspace"
                                                                                     : "/workspace",
                                                                                 new EmptyStruct()
                                                                             }
                                                                         }
                                                                     } )
                                                                     {
                                                                         HostConfig = new HostConfig
                                                                         {
                                                                             Binds = new[]
                                                                             {
                                                                                 job.Environment == BuildEnvironment.Windows
                                                                                     ? $@"{path}:c:\workspace"
                                                                                     : $"{path}:/workspace"
                                                                             }
                                                                         }
                                                                     } );


            var multiplexedStream = await dockerClient.Containers.AttachContainerAsync( container.ID, true, new ContainerAttachParameters
            {
                Stdout = true,
                Stderr = true,
                Stream = true
            } );

            await dockerClient.Containers.StartContainerAsync( container.ID, new ContainerStartParameters() );
            await StreamTty( multiplexedStream );
            await dockerClient.Containers.WaitContainerAsync( container.ID );
            OnProgress?.Invoke( this, new JobProgressDto
            {
                ProgressType = ProgressType.Success,
                Message = "Build completed"
            } );
        }

        private void ReportDockerPullProgress( JSONMessage message )
        {
            if ( message.ID.IsNullOrEmpty() || message.ID == "latest" )
            {
                return;
            }

            if ( message.Progress == null )
                return;

            var total = (int) ( message.Progress?.Total == 0 ? 100 : message.Progress?.Total );
            int current = message.Status == "Download complete" ? total : (int) message.Progress?.Current;

            var progressBar = progressBars.ContainsKey( message.ID )
                ? progressBars[ message.ID ]
                : new ProgressBar( 100 );

            progressBar.Max = total;
            progressBar.Refresh( current, $"{message.ID}: {message.Status}" );

            progressBars[ message.ID ] = progressBar;
        }

        private Credentials GetDockerRemoteApiCredentials()
        {
            var uri = new Uri( dockerConfiguration.RemoteEndpoint );

            if ( uri.Scheme == "npipe" || uri.Scheme == "unix" )
            {
                return new AnonymousCredentials();
            }

            if ( !dockerConfiguration.PfxPath.IsNullOrWhiteSpace() )
            {
                return dockerConfiguration.PfxPassword.IsNullOrEmpty()
                    ? new CertificateCredentials( new X509Certificate2( dockerConfiguration.PfxPath ) )
                    : new CertificateCredentials( new X509Certificate2( dockerConfiguration.PfxPath, dockerConfiguration.PfxPassword ) );
            }

            if ( dockerConfiguration.PfxBytes != null && dockerConfiguration.PfxBytes.Length > 0 )
            {
                return dockerConfiguration.PfxPassword.IsNullOrEmpty()
                    ? new CertificateCredentials( new X509Certificate2( dockerConfiguration.PfxBytes ) )
                    : new CertificateCredentials( new X509Certificate2( dockerConfiguration.PfxBytes, dockerConfiguration.PfxPassword ) );
            }

            if ( dockerConfiguration.BasicAuth != null &&
                 !dockerConfiguration.BasicAuth.Username.IsNullOrWhiteSpace() &&
                 !dockerConfiguration.BasicAuth.Password.IsNullOrWhiteSpace() )
            {
                return new BasicAuthCredentials( dockerConfiguration.BasicAuth.Username, dockerConfiguration.BasicAuth.Password, dockerConfiguration.BasicAuth.Tls );
            }

            return new AnonymousCredentials();
        }

        private async Task<string> ReadLineFromStream( MultiplexedStream multiplexedStream )
        {
            var data = new List<byte>();
            var buffer = new byte[ 1 ];
            string line = null;

            while ( !( await multiplexedStream.ReadOutputAsync( buffer, 0, buffer.Length, CancellationToken.None ) ).EOF )
            {
                data.Add( buffer[ 0 ] );
                if ( (char) buffer[ 0 ] != '\n' )
                    continue;
                line = Encoding.UTF8.GetString( data.ToArray() ).Trim( '\r', '\n' );
                break;
            }

            return line;
        }

        private async Task StreamTty( MultiplexedStream multiplexedStream )
        {
            string line;
            while ( ( line = await ReadLineFromStream( multiplexedStream ) ) != null )
            {
                OnProgress?.Invoke( this, new JobProgressDto
                {
                    ProgressType = ProgressType.Informational,
                    Message = line
                } );
            }
        }

        private async Task GenerateStepScripts( JobDto job, BuildFile buildFile, string path )
        {
            string fileExtension = job.Environment == BuildEnvironment.Windows
                ? "ps1"
                : "sh";

            string filename = $"{path}/coreci_build_steps.{fileExtension}";

            using ( TextWriter tw = File.CreateText( filename ) )
            {
                if ( job.Environment == BuildEnvironment.Windows )
                {
                    tw.NewLine = "\r\n";

                    await tw.WriteLineAsync( "Set-PSDebug -Trace 1" );
                    await tw.WriteLineAsync( @"cd c:\workspace" );
                }
                else
                {
                    tw.NewLine = "\n";
                    await tw.WriteLineAsync( "#!/bin/sh" );
                    await tw.WriteLineAsync( "set -x" );
                    await tw.WriteLineAsync( @"cd /workspace" );
                }

                foreach ( string command in buildFile.Commands )
                {
                    await tw.WriteLineAsync( $"{command};" );
                }
            }
        }
    }
}
