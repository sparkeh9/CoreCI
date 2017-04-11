namespace CoreCI.Kernel.Tests.PipelineProcessorTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Extensions;
    using Infrastructure.Helpers;
    using Microsoft.Extensions.Options;
    using Models;
    using PipelineProcessor;
    using Shouldly;
    using Xunit;

    public class When_initiating_a_pipeline_process
    {
        private static PipelineProcessor SystemUnderTest( Stream inStream = null, Stream outStream = null, Stream errStream = null )
        {
            return new PipelineProcessor( Options.Create( new PipelineProcessorOptions
            {
                Workspace = Path.Combine( ResourceHelpers.GetTempFilePath(), "workspaces" ),
                PfxPath = @"C:\Users\spark\.docker\machine\certs\key.pfx"
            } ), inStream, outStream, errStream );
        }

        [ Fact ]
        public void Should_cancel_if_token_cancellation_requested()
        {
            var sut = SystemUnderTest();
            var cts = new CancellationTokenSource();
            cts.Cancel();

            Should.Throw<OperationCanceledException>( () => sut.ProcessAsync( new Pipeline(), cts.Token ) );
        }

        [ Fact ]
        public async Task Should_create_workspace_for_process()
        {
            var guid = Guid.Empty;
            var sut = SystemUnderTest();
            try
            {
                var result = await sut.ProcessAsync( new Pipeline() );
                guid = result.Identitifer;
                result.WorkspacePath.ShouldNotBeNullOrWhiteSpace();

                Directory.Exists( result.WorkspacePath ).ShouldBeTrue();
            }
            finally
            {
                await sut.CleanupWorkspaceAsync( guid );
            }
        }

        [ Fact ]
        public async Task Should_run_steps_in_a_process()
        {
//            var inputStream = new MemoryStream();
            var outputStream = new MemoryStream();
            var errorStream = new MemoryStream();

            var guid = Guid.Empty;
            var sut = SystemUnderTest( outStream: outputStream, errStream: errorStream );
            try
            {
                string command = $"test from {nameof(Should_run_steps_in_a_process)}";
                var result = await sut.ProcessAsync( new Pipeline
                {
                    Steps = new List<Step>
                    {
                        new Step
                        {
                            Name = nameof(Should_run_steps_in_a_process),
                            Image = new Image
                            {
                                Parent = "alpine",
                                Tag = "latest"
                            },
                            Commands = new List<string>
                            {
                                "/bin/sh",
                                "-c",
                                $"echo {command}"
                            }
                        }
                    }
                } );


                string output = outputStream.DumpToString();

                output.ShouldContain( command );
            }
            catch ( Exception e )
            {
                throw;
            }
            finally
            {
                if ( guid != default( Guid ) )
                    await sut.CleanupWorkspaceAsync( guid );
            }
        }
    }
}