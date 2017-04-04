namespace CoreCI.Kernel.Tests.PipelineProcessorTests
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Helpers;
    using Microsoft.Extensions.Options;
    using Models;
    using PipelineProcessor;
    using Shouldly;
    using Xunit;

    public class When_initiating_a_pipeline_process
    {
        private static PipelineProcessor SystemUnderTest()
        {
            return new PipelineProcessor( Options.Create( new PipelineProcessorOptions
            {
                Workspace = Path.Combine( ResourceHelpers.GetTempFilePath(), "workspaces" )
            } ) );
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
    }
}