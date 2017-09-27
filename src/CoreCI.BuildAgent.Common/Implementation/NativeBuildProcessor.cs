namespace CoreCI.BuildAgent.Common.Implementation
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using CoreCI.Common.Models;
    using CoreCI.Common.Models.Jobs;
    using Models.BuildFile;

    public class NativeBuildProcessor : IBuildProcessor
    {
        public event EventHandler<JobProgressDto> OnProgress;

        public async Task DoBuildAsync( JobDto job, BuildFile buildFile, string path )
        {
            foreach ( string command in buildFile.Commands )
            {
                await RunShellCommand( job.Environment, path, command );
            }
        }

        private Task RunShellCommand( BuildEnvironment environment, string path, string command )
        {
            var tcs = new TaskCompletionSource<object>();
            string shellPath = environment == BuildEnvironment.Windows
                ? "powershell.exe"
                : "/bin/bash";

            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    FileName = shellPath,
                    Arguments = command,
                    WorkingDirectory = path,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true
                }
            };

            OnProgress?.Invoke( this, new JobProgressDto
            {
                JobProgressType = JobProgressType.Command,
                Message = command
            } );

            process.OutputDataReceived += ( sender, args ) =>
                                          {
                                              OnProgress?.Invoke( this, new JobProgressDto
                                              {
                                                  JobProgressType = JobProgressType.Informational,
                                                  Message = args.Data
                                              } );
                                          };

            process.ErrorDataReceived += ( sender, args ) =>
                                         {
                                             OnProgress?.Invoke( this, new JobProgressDto
                                             {
                                                 JobProgressType = JobProgressType.Error,
                                                 Message = args.Data
                                             } );
                                         };
            process.Exited += ( sender, args ) => { tcs.SetResult( null ); };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
        }
    }
}
