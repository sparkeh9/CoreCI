namespace CoreCI.BuildAgent.Common.Implementation
{
    using System;
    using System.Diagnostics;
    using CoreCI.Common.Extensions;
    using CoreCI.Common.Models;
    using CoreCI.Common.Models.Jobs;
    using Models.BuildFile;

    public class BuildProcessor : IBuildProcessor
    {
        public event EventHandler<string> OnProgress;

        public void DoBuild( JobDto job, BuildFile buildFile, string path )
        {
            if ( buildFile.BuildMode == BuildMode.Native )
            {
                DoNativeBuild( job, buildFile, path );
            }
        }

        private void DoNativeBuild( JobDto job, BuildFile buildFile, string path )
        {
            foreach ( var command in buildFile.Commands )
            {
                RunShellCommand( job.Environment, path, command );
            }
        }

        private void RunShellCommand( BuildEnvironment environment, string path, string command )
        {
            string shellPath = environment == BuildEnvironment.Windows
                ? "powershell.exe"
                : "/bin/bash";

            var process = new Process
            {
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

            OnProgress?.Invoke( this, command );
            process.OutputDataReceived += ( sender, args ) => { OnProgress?.Invoke( this, args.Data ); };
            process.ErrorDataReceived += ( sender, args ) => { OnProgress?.Invoke( this, args.Data ); };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }
    }
}
