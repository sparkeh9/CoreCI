namespace CoreCI.BuildAgent.Common.BuildAgentCore.BuildProcessor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using BuildFile;
    using CoreCI.Common.Models;
    using CoreCI.Common.Models.Jobs;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Models.BuildFile;
    using Polly;
    using Polly.Retry;
    using Progress;
    using Sdk;
    using Vcs;

    public class BuildAgentDaemon : IBuildAgentDaemon
    {
        private readonly IBuildFileParser buildFileParser;
        private readonly ICoreCI coreCiClient;
        private readonly DockerBuildProcessor dockerBuildProcessor;
        private readonly DockerClient dockerClient;
        private readonly NativeBuildProcessor nativeBuildProcessor;
        private readonly IBuildProgressReporter progressReporter;
        private readonly IVcsAppropriator vcsAppropriator;
        private readonly RetryPolicy waitForeverPolicy;
        private IBuildProcessor buildProcessor;

        private SystemInfoResponse dockerSystemInfoResponse;
        private List<BuildEnvironment> environmentMatrix;

        private bool isRegistered;

        public BuildAgentDaemon( ICoreCI coreCiClient, IVcsAppropriator vcsAppropriator, IBuildFileParser buildFileParser, IBuildProgressReporter progressReporter, DockerBuildProcessor dockerBuildProcessor, NativeBuildProcessor nativeBuildProcessor,
                                 DockerClient dockerClient )
        {
            this.coreCiClient = coreCiClient;
            this.vcsAppropriator = vcsAppropriator;
            this.buildFileParser = buildFileParser;
            this.progressReporter = progressReporter;
            this.dockerBuildProcessor = dockerBuildProcessor;
            this.nativeBuildProcessor = nativeBuildProcessor;
            this.dockerClient = dockerClient;

            this.vcsAppropriator.OnProgress += ( sender, report ) =>
                                               {
                                                   try
                                                   {
                                                       progressReporter.ReportAsync( report );
                                                   }
                                                   catch ( OperationCanceledException ) { }
                                               };
            waitForeverPolicy = Policy.Handle<Exception>()
                                      .WaitAndRetryForeverAsync( Sleep, onRetry : async ( exception, span ) => { await OnRetryAsync( exception, span ); } );
        }

        public event EventHandler<bool> PollStatusChanged;

        public async Task InvokeAsync( BuildEnvironmentOs environment )
        {
            try
            {
                DisablePolling();
                await DetermineBuildEnvironmentMatrix( environment );
                await RegisterWithCoordinator( environmentMatrix );
                await progressReporter.ReportAsync( new JobProgressDto( "Checking for available jobs", JobProgressType.Command ) );

                var reserveResult = await coreCiClient.Jobs.ReserveFirstAvailableJobAsync( environmentMatrix );

                if ( !reserveResult.HasValue )
                {
                    EnablePolling();
                    return;
                }

                progressReporter.UseBuildAgentToken( reserveResult.Value.reservation.BuildAgentToken );

                string tempPath = GenerateTempPath();

                await progressReporter.ReportAsync( new JobProgressDto
                {
                    Message = $"Reserved job {reserveResult.Value.job.JobId}",
                    JobProgressType = JobProgressType.Informational
                } );

                await vcsAppropriator.AcquireAsync( reserveResult.Value.job, tempPath );
                var buildFile = buildFileParser.ParseBuildFile( tempPath );
                InitialiseBuildProcessor( buildFile );
                await buildProcessor.DoBuildAsync( reserveResult.Value.job, buildFile, tempPath );


            }
            catch ( Exception e )
            {
                await progressReporter.ReportAsync( new JobProgressDto
                {
                    Message = e.Message,
                    JobProgressType = JobProgressType.Error
                } );
            }
            finally
            {
                if ( buildProcessor != null )
                {
                    buildProcessor.OnProgress -= BuildProcessorOnProgress;
                }
                EnablePolling();
            }
        }

        public async Task StopAsync()
        {
            DisablePolling();

            if ( !isRegistered )
            {
                return;
            }

            try
            {
                await coreCiClient.Agents.DeregisterAsync();
            }
            catch ( Exception e )
            {
                Console.WriteLine( e.Message );
            }
        }

        private async Task RegisterWithCoordinator( List<BuildEnvironment> environments )
        {
            if ( !isRegistered )
            {
                await waitForeverPolicy.ExecuteAsync( async () =>
                                                      {
                                                          await progressReporter.ReportAsync( new JobProgressDto( "Attempting to register build agent with coordinator", JobProgressType.Informational, false ) );
                                                          await coreCiClient.Agents.RegisterAsync( environments );
                                                          await progressReporter.ReportAsync( new JobProgressDto( "Registered with coordinator", JobProgressType.Informational, false ) );
                                                          isRegistered = true;
                                                      } );
            }
        }

        private async Task DetermineBuildEnvironmentMatrix( BuildEnvironmentOs nativeOs )
        {
            if ( environmentMatrix == null )
            {
                try
                {
                    var cts = new CancellationTokenSource();
                    cts.CancelAfter( TimeSpan.FromSeconds( 10 ) );
                    dockerSystemInfoResponse = await dockerClient.System.GetSystemInfoAsync( cts.Token );
                }
                catch ( Exception )
                {
                    // ignored
                }

                environmentMatrix = new List<BuildEnvironment>
                {
                    new BuildEnvironment
                    {
                        BuildOs = nativeOs,
                        BuildMode = BuildMode.Native
                    }
                };

                if ( dockerSystemInfoResponse != null )
                {
                    environmentMatrix.Add( new BuildEnvironment
                    {
                        BuildMode = BuildMode.Docker,
                        BuildOs = dockerSystemInfoResponse.OSType == "linux"
                            ? BuildEnvironmentOs.Linux
                            : BuildEnvironmentOs.Windows
                    } );
                }
            }
        }

        private void InitialiseBuildProcessor( BuildFile buildFile )
        {
            if ( buildFile.BuildMode == BuildMode.Native )
            {
                buildProcessor = nativeBuildProcessor;
            }
            else
            {
                buildProcessor = dockerBuildProcessor;
            }

            buildProcessor.OnProgress -= BuildProcessorOnProgress;
            buildProcessor.OnProgress += BuildProcessorOnProgress;
        }

        private void BuildProcessorOnProgress( object o, JobProgressDto report )
        {
            progressReporter.ReportAsync( report );
        }

        private async Task OnRetryAsync( Exception exception, TimeSpan calculatedWaitDuration )
        {
            DisablePolling();
            await progressReporter.ReportAsync( new JobProgressDto
            {
                Message = $"{exception.Message} - retrying in {calculatedWaitDuration}",
                JobProgressType = JobProgressType.Error
            } );
        }

        private static TimeSpan Sleep( int attempt )
        {
            return TimeSpan.FromSeconds( 30 );
        }

        private void DisablePolling()
        {
            PollStatusChanged?.Invoke( this, false );
        }

        private void EnablePolling()
        {
            PollStatusChanged?.Invoke( this, true );
        }

        private static string GenerateTempPath()
        {
            return Path.GetFullPath( string.Join( $"{Path.DirectorySeparatorChar}", new[]
            {
                Path.GetTempPath(),
                "coreci",
                Guid.NewGuid().ToString( "D" )
            } ) );
        }
    }
}
