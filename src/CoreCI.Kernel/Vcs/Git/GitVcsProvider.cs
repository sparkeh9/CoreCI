namespace CoreCI.Kernel.Vcs.Git
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Extensions;
    using Models;
    using PipelineProcessor;

    public class GitVcsProvider
    {
        private readonly IPipelineProcessor pipelineProcessor;

        public GitVcsProvider( IPipelineProcessor pipelineProcessor )
        {
            this.pipelineProcessor = pipelineProcessor;
        }

        public async Task<PipelineProcessorResult> AcquireAsync( GitVcsConfiguration configuration, CancellationToken ctx = default(CancellationToken) )
        {
            return await pipelineProcessor.ProcessAsync( new Pipeline
            {
                WorkspacePath = configuration.WorkspacePath,
                Steps = new List<Step>
                {
                    new Step
                    {
                        Name = "CloneGitRepository",
                        Image = new Image
                        {
                            Parent = "bravissimolabs/alpine-git",
                            Tag = "latest"
                        },
                        Commands = GenerateCommands( configuration )
                    }
                }
            }, ctx );
        }

        internal IEnumerable<string> GenerateCommands( GitVcsConfiguration configuration )
        {
            yield return "git init";

            if ( !configuration.Sha.IsNullOrWhiteSpace() )
            {
                yield return $"git remote add origin {configuration.RemoteUrl}";
                yield return "git pull";
                yield return $"git checkout -qf {configuration.Sha}";
                yield break;
            }

            if ( configuration.Branch.IsNullOrWhiteSpace() )
            {
                yield return $"git remote add origin {configuration.RemoteUrl}";
                yield return "git fetch origin master --depth=1";
                yield return "git checkout -qf master";
            }
            else
            {
                yield return $"git remote add -t {configuration.Branch} -f origin {configuration.RemoteUrl}";
                yield return $"git checkout -qf origin/{configuration.Branch}";
            }
        }
    }
}