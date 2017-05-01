namespace CoreCI.Kernel.Tests.VcsTests.Git
{
    using System.IO;
    using System.Linq;
    using Infrastructure.Helpers;
    using Shouldly;
    using Vcs.Git;
    using Xunit;
    using Xunit.Abstractions;

    public class When_generating_git_commands : TestBase
    {
        public When_generating_git_commands( ITestOutputHelper outputHelper ) : base( outputHelper ) { }

        private static GitVcsProvider SystemUnderTest( Stream inStream = null, Stream outStream = null, Stream errStream = null )
        {
            return new GitVcsProvider( PipelinePricessor( inStream, outStream, errStream ) );
        }

        private static PipelineProcessor.PipelineProcessor PipelinePricessor( Stream inStream = null, Stream outStream = null, Stream errStream = null )
        {
            return new PipelineProcessor.PipelineProcessor( PipelineProcessorOptionsHelper.GenerateOptionsForTest(), inStream, outStream, errStream );
        }

        [ Fact ]
        public void Should_generate_clone_without_branch()
        {
            var sut = SystemUnderTest();
            var commands = sut.GenerateCommands( new GitVcsConfiguration
            {
                RemoteUrl = "https://github.com/sparkeh9/CoreCI.git",
                WorkspacePath = "/workspace"
            } );


            commands.Count().ShouldBe( 4 );
            commands.ElementAt( 2 ).ShouldBe( "git fetch origin master --depth=1" );
        }

        [ Fact ]
        public void Should_generate_clone_with_branch()
        {
            var sut = SystemUnderTest();
            var commands = sut.GenerateCommands( new GitVcsConfiguration
            {
                Branch = "master",
                RemoteUrl = "https://github.com/sparkeh9/CoreCI.git",
                WorkspacePath = "/workspace"
            } );


            commands.Count().ShouldBe( 3 );
            commands.ElementAt( 1 ).ShouldBe( "git remote add -t master -f origin https://github.com/sparkeh9/CoreCI.git" );
        }

        [ Fact ]
        public void Should_generate_sha_checkout()
        {
            var sut = SystemUnderTest();
            var commands = sut.GenerateCommands( new GitVcsConfiguration
            {
                Branch = "master",
                RemoteUrl = "https://github.com/sparkeh9/CoreCI.git",
                WorkspacePath = "/workspace",
                Sha = "ABC123"
            } );

            commands.Count().ShouldBe( 4 );
            commands.ElementAt( 3 ).ShouldBe( "git checkout -qf ABC123" );
        }
    }
}