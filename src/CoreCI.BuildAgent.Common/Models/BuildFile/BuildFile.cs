namespace CoreCI.BuildAgent.Common.Models.BuildFile
{
    using System.Collections.Generic;
    using CoreCI.Common.Models;
    using YamlDotNet.Serialization;

    public class BuildFile
    {
        public BuildEnvironment Environment { get; set; }

        [ YamlMember( Alias = "mode" ) ]
        public BuildMode BuildMode { get; set; }

        [ YamlMember( Alias = "image" ) ]
        public DockerImage DockerImage { get; set; }

        public IEnumerable<string> Commands { get; set; } = new List<string>();
    }
}
