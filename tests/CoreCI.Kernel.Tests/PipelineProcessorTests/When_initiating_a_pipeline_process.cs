namespace CoreCI.Kernel.Tests.PipelineProcessorTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Models;
    using Infrastructure.Helpers;
    using Kernel.Infrastructure.Extensions;
    using Shouldly;
    using Xunit;
    using Xunit.Abstractions;

    public class When_initiating_a_pipeline_process : TestBase
    {
        public When_initiating_a_pipeline_process( ITestOutputHelper outputHelper ) : base( outputHelper ) { }

        private PipelineProcessor.PipelineProcessor SystemUnderTest( Stream inStream = null, Stream outStream = null, Stream errStream = null )
        {
            return new PipelineProcessor.PipelineProcessor( PipelineProcessorOptionsHelper.GenerateOptionsForTest(), inStream, outStream, errStream )
            {
                Logger = this.Logger
            };
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
                var result = await sut.ProcessAsync( new Pipeline
                {
                    Steps = new List<Step>
                    {
                        new Step
                        {
                            Name = "DummyStep",
                            Image = TestStepHelper.BasicAlpineImage(),
                            Commands = TestStepHelper.DummyCommand()
                        }
                    }
                } );
                guid = result.Identitifer.GetValueOrDefault();
                result.WorkspacePath.ShouldNotBeNullOrWhiteSpace();

                Directory.Exists( result.WorkspacePath ).ShouldBeTrue();
            }
            finally
            {
                await sut.CleanupWorkspaceAsync( guid );
            }
        }

        [ Fact ]
        public async Task Should_create_script_based_on_entered_commands()
        {
            var guid = Guid.Empty;
            var sut = SystemUnderTest();
            try
            {
                var result = await sut.ProcessAsync( new Pipeline
                {
                    Steps = new List<Step>
                    {
                        new Step
                        {
                            Name = "MyStep",
                            Image = TestStepHelper.BasicAlpineImage(),
                            Commands = TestStepHelper.DummyCommand()
                        }
                    }
                } );

                guid = result.Identitifer.GetValueOrDefault();
                result.WorkspacePath.ShouldNotBeNullOrWhiteSpace();

                File.Exists( $"{result.WorkspacePath}/MyStep_{guid}.sh" ).ShouldBeTrue();
            }
            finally
            {
                await sut.CleanupWorkspaceAsync( guid );
            }
        }

        [ Fact ]
        public async Task Should_run_steps_in_a_process()
        {
            var outputStream = new MemoryStream();
            var errorStream = new MemoryStream();

            var guid = Guid.Empty;
            var sut = SystemUnderTest( outStream: outputStream, errStream: errorStream );
            try
            {
                var command = $"test from {nameof( Should_run_steps_in_a_process )}";
                var result = await sut.ProcessAsync( new Pipeline
                {
                    Steps = new List<Step>
                    {
                        new Step
                        {
                            Name = nameof( Should_run_steps_in_a_process ),
                            Image = TestStepHelper.BasicAlpineImage(),
                            Commands = new List<string>
                            {
                                $"echo {command}"
                            }
                        }
                    }
                } );


                var output = outputStream.DumpToString();
                var errors = errorStream.DumpToString();

                output.ShouldContain( command );
                errors.ShouldBeEmpty();
            }
            catch ( Exception e )
            {
                throw;
            }
            finally
            {
                if ( guid != default(Guid) )
                    await sut.CleanupWorkspaceAsync( guid );
            }
        }
    }
}