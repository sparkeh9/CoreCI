namespace CoreCI.Kernel.Tests.VcsTests.Git
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Helpers;
    using Vcs.Git;
    using Shouldly;
    using Xunit;

    public class When_acquiring_a_repository
    {
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

            var guid = Guid.Empty;
            var sut = SystemUnderTest( outStream: outputStream, errStream: errorStream );
            try
            {
                var result = await sut.AcquireAsync( new GitVcsConfiguration
                {
                    RemoteUrl = "https://github.com/sparkeh9/Demo.git",
                    Ref = "refs/heads/master",
                    Sha = "1ddd6f6a59c0de473e4092d66a87beff13968320"
                } );
            }
            catch ( Exception e )
            {
                throw;
            }
        }
    }
}