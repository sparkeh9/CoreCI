namespace CoreCI.Kernel.Vcs.Git
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Models;
    using PipelineProcessor;

    public class GitVcsProvider
    {
        private readonly IPipelineProcessor pipelineProcessor;

        public GitVcsProvider( IPipelineProcessor pipelineProcessor )
        {
            this.pipelineProcessor = pipelineProcessor;
        }

        public async Task<bool> AcquireAsync( GitVcsConfiguration configuration, CancellationToken ctx = default( CancellationToken ) )
        {
            var result = await pipelineProcessor.ProcessAsync( new Pipeline
            {
                WorkspacePath = configuration.WorkspacePath,
                Steps = new List<Step>
                {
                    new Step
                    {
                        Name = "CloneGitRepository",
                        Image = new Image
                        {
                            Parent = "plugins/git",
                            Tag = "latest"
                        },
                        EnvironmentVariables = new Dictionary<string, string>
                        {
                            { "DRONE_WORKSPACE", "/checkout" },
                            { "DRONE_REMOTE_URL", configuration.RemoteUrl },
                            { "DRONE_COMMIT_SHA", configuration.Sha },
                            { "DRONE_COMMIT_REF", configuration.Ref },
                        },
                        Volumes = new Dictionary<string, string>
                        {
                            { configuration.WorkspacePath, "/checkout" }
                        }
                    }
                }
            }, ctx );

            return true;
        }
    }
}