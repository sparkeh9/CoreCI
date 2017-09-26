namespace CoreCI.BuildAgent.Common.Implementation
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using CoreCI.Common.Models;
    using Models.BuildFile;
    using Models.Docker;

    public class DockerBuildScriptGenerator : IDockerBuildScriptGenerator
    {
        public async Task GenerateBuildScript( BuildFile buildFile, DockerJobEnvironmentConfiguration dockerEnvironmentConfig )
        {
            using ( TextWriter tw = File.CreateText( dockerEnvironmentConfig.HostWorkspaceBuildFilePath ) )
            {
                tw.NewLine = dockerEnvironmentConfig.NewLineCharacter;
                if ( buildFile.Environment == BuildEnvironment.Windows )
                {
                    await tw.WriteLineAsync( @"cd c:\workspace" );
                }
                else
                {
                    await tw.WriteLineAsync( "#!/bin/sh" );
                    await tw.WriteLineAsync( @"cd /workspace" );
                }

                foreach ( string command in GenerateCommands( buildFile.Commands ) )
                {
                    await tw.WriteLineAsync( $"{command};" );
                }
            }
        }

        private IEnumerable<string> GenerateCommands( IEnumerable<string> input )
        {
            foreach ( string command in input )
            {
                yield return $"echo '{command}'";
                yield return command;
            }
        }
    }
}
