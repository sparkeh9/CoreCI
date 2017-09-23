﻿namespace CoreCI.BuildAgent.Common.Models.BuildFile
{
    using System.Collections.Generic;
    using YamlDotNet.Serialization;

    public class BuildFile
    {
        [ YamlMember( Alias = "mode" ) ]
        public BuildMode BuildMode { get; set; }

        [ YamlMember( Alias = "image" ) ]
        public DockerImage DockerImage { get; set; }

        public IEnumerable<string> Commands { get; set; } = new List<string>();
    }
}
