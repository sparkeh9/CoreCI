namespace CoreCI.BuildAgent.Common.BuildAgentCore.BuildProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CoreCI.Common.Extensions;
    using CoreCI.Common.Models.Jobs;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Extentions;
    using Konsole;
    using Models.BuildFile;
    using Models.Docker;

    public class DockerBuildProcessor : IBuildProcessor
    {
        private readonly DockerClient dockerClient;
        private readonly Dictionary<string, ProgressBar> progressBars = new Dictionary<string, ProgressBar>();
        private readonly IDockerBuildScriptGenerator scriptGenerator;

        public DockerBuildProcessor( DockerClient dockerClient, IDockerBuildScriptGenerator scriptGenerator )
        {
            this.scriptGenerator = scriptGenerator;
            this.dockerClient = dockerClient;
        }

        public event EventHandler<JobProgressDto> OnProgress;

        public async Task<JobCompletionResult> DoBuildAsync( JobDto job, BuildFile buildFile, string path )
        {
            var dockerEnvironmentConfig = new DockerJobEnvironmentConfiguration( job, buildFile, path );
            await scriptGenerator.GenerateBuildScript( buildFile, dockerEnvironmentConfig );
            await PullMessageIfNecessaryAsync( buildFile );
            var container = await CreateContainerAsync( buildFile, dockerEnvironmentConfig );
            await StartContainerAndListenAsync( container );
            await dockerClient.Containers.WaitContainerAsync( container.ID );

            OnProgress?.Invoke( this, new JobProgressDto
            {
                JobProgressType = JobProgressType.Success,
                Message = "Build completed"
            } );


            return new JobCompletionResult
            {
                Successful = true
            };
        }

        private async Task StartContainerAndListenAsync( CreateContainerResponse container )
        {
            var multiplexedStream = await dockerClient.Containers.AttachContainerAsync( container.ID, true, new ContainerAttachParameters
            {
                Stdout = true,
                Stderr = true,
                Stream = true
            } );

            await dockerClient.Containers.StartContainerAsync( container.ID, new ContainerStartParameters() );
            await StreamTty( multiplexedStream );
        }

        private async Task<CreateContainerResponse> CreateContainerAsync( BuildFile buildFile, DockerJobEnvironmentConfiguration dockerEnvironmentConfig )
        {
            OnProgress?.Invoke( this, new JobProgressDto
            {
                JobProgressType = JobProgressType.Command,
                Message = $"Creating container from image {buildFile.DockerImage}"
            } );

            var container = await dockerClient.Containers
                                              .CreateContainerAsync( new CreateContainerParameters( new Config
                                                                     {
                                                                         Image = buildFile.DockerImage.ToString(),

                                                                         AttachStdout = true,
                                                                         AttachStderr = true,
                                                                         Entrypoint = new List<string>
                                                                         {
                                                                             dockerEnvironmentConfig.Shell,
                                                                             dockerEnvironmentConfig.FileName
                                                                         },
//                                  Env = step.EnvironmentVariables?.Select( x => $"{x.Key}={x.Value}" ).ToList(),
                                                                         Volumes = new Dictionary<string, EmptyStruct>
                                                                         {
                                                                             {
                                                                                 dockerEnvironmentConfig.InsideContainerDirectory,
                                                                                 new EmptyStruct()
                                                                             }
                                                                         }
                                                                     } )
                                                                     {
                                                                         HostConfig = new HostConfig
                                                                         {
                                                                             Binds = new[]
                                                                             {
                                                                                 dockerEnvironmentConfig.DockerVolumeBinding
                                                                             }
                                                                         }
                                                                     } );
            return container;
        }

        private async Task PullMessageIfNecessaryAsync( BuildFile buildFile )
        {
            var foundImage = await dockerClient.Images.ListImagesAsync( new ImagesListParameters
            {
                All = false,
                MatchName = buildFile.DockerImage.ToString()
            } );

            if ( !foundImage.Any() )
            {
                OnProgress?.Invoke( this, new JobProgressDto
                {
                    JobProgressType = JobProgressType.Command,
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
        }

        private void ReportDockerPullProgress( JSONMessage message )
        {
            if ( message.ID.IsNullOrEmpty() || message.ID == "latest" )
            {
                return;
            }

            if ( message.Progress == null )
            {
                return;
            }

            var total = (int) ( message.Progress?.Total == 0 ? 100 : message.Progress?.Total );
            int current = message.Status == "Download complete" ? total : (int) message.Progress?.Current;

            var progressBar = progressBars.ContainsKey( message.ID )
                ? progressBars[ message.ID ]
                : new ProgressBar( 100 );

            progressBar.Max = total;
            progressBar.Refresh( current, $"{message.ID}: {message.Status}" );

            progressBars[ message.ID ] = progressBar;
        }

        private async Task StreamTty( MultiplexedStream multiplexedStream )
        {
            string line;
            while ( ( line = await multiplexedStream.ReadLineAsync() ) != null )
            {
                OnProgress?.Invoke( this, new JobProgressDto
                {
                    JobProgressType = JobProgressType.Informational,
                    Message = line
                } );
            }
        }
    }
}
