namespace CoreCI.Kernel.Tests.VcsTests.Git
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Helpers;
    using Kernel.Infrastructure.Extensions;
    using Microsoft.Extensions.Logging;
    using Vcs.Git;
    using Shouldly;
    using Xunit;
    using Xunit.Abstractions;

    public class When_acquiring_a_repository : TestBase
    {
        public When_acquiring_a_repository( ITestOutputHelper outputHelper ) : base( outputHelper ) { }

        private static GitVcsProvider SystemUnderTest( Stream inStream = null, Stream outStream = null, Stream errStream = null )
        {
            return new GitVcsProvider( PipelinePricessor( inStream, outStream, errStream ) );
        }

        private static PipelineProcessor.PipelineProcessor PipelinePricessor( Stream inStream = null, Stream outStream = null, Stream errStream = null )
        {
            return new PipelineProcessor.PipelineProcessor( PipelineProcessorOptionsHelper.GenerateOptionsForTest(), inStream, outStream, errStream );
        }

        [ Fact ]
        public void Should_cancel_if_token_cancellation_requested()
        {
            var sut = SystemUnderTest();
            var cts = new CancellationTokenSource();
            cts.Cancel();

            Should.Throw<OperationCanceledException>( () => sut.AcquireAsync( new GitVcsConfiguration(), cts.Token ) );
        }

        [ Fact ]
        public async Task Should_place_acquired_repository_in_workspace()
        {
            var outputStream = new MemoryStream();
            var errorStream = new MemoryStream();
            
            var sut = SystemUnderTest( outStream: outputStream, errStream: errorStream );
            try
            {
                var result = await sut.AcquireAsync( new GitVcsConfiguration
                {
                    RemoteUrl = "https://github.com/sparkeh9/EnchiladaDemo.git",
                    Branch = "master",
                    WorkspacePath = "/workspace"
                } );

                var output = outputStream.DumpToString();
                result.WorkspacePath.ShouldNotBeNull();

                Directory.GetFiles( result.WorkspacePath ).Contains( "LICENSE" );
            }
            catch ( Exception e )
            {
                throw;
            }
        }
    }
}