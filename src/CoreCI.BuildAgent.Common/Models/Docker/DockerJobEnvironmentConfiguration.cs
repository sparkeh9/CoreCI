namespace CoreCI.BuildAgent.Common.Models.Docker
{
    using BuildFile;
    using CoreCI.Common.Models;
    using CoreCI.Common.Models.Jobs;

    public class DockerJobEnvironmentConfiguration
    {
        public string HostWorkspaceBuildFilePath { get; set; }
        public string FileName { get; set; }
        public string Shell { get; set; }
        public string InsideContainerDirectory { get; set; }
        public string DockerVolumeBinding { get; set; }
        public string NewLineCharacter { get; set; }
        public string ScriptFileExtension { get; set; }

        public DockerJobEnvironmentConfiguration( JobDto job, BuildFile buildFile, string path )
        {
            if ( buildFile.Environment == BuildEnvironmentOs.Windows )
            {
                ScriptFileExtension = "ps1";
                InsideContainerDirectory = @"c:\workspace";
                FileName = $@"{InsideContainerDirectory}\coreci_build_steps.{ScriptFileExtension}";
                Shell = @"powershell";
                DockerVolumeBinding = $@"{path}:c:\workspace";
                NewLineCharacter = "\r\n";
            }
            else
            {
                ScriptFileExtension = "sh";
                InsideContainerDirectory = @"/workspace";
                FileName = $@"{InsideContainerDirectory}/coreci_build_steps.{ScriptFileExtension}";
                Shell = @"sh";
                DockerVolumeBinding = $@"{path}:/workspace";
                NewLineCharacter = "\n";
            }
            
            HostWorkspaceBuildFilePath = job.Environment == BuildEnvironmentOs.Windows
                ? $@"{path}\coreci_build_steps.{ScriptFileExtension}"
                : $@"{path}/coreci_build_steps.{ScriptFileExtension}";
        }
    }
}
