namespace CoreCI.BuildAgent.Common.Implementation
{
    using System.IO;
    using Exceptions;
    using Models.BuildFile;
    using YamlDotNet.Serialization;

    public class BuildFileParser : IBuildFileParser
    {
        private readonly Deserializer deserialiser;

        public BuildFileParser( Deserializer deserialiser )
        {
            this.deserialiser = deserialiser;
        }

        public BuildFile ParseBuildFile( string path )
        {
            string filePath = $"{path}{Path.DirectorySeparatorChar}coreci.yml";
            if ( !File.Exists( filePath ) )
            {
                throw new NoBuildFileFoundException();
            }

            string text = File.ReadAllText( filePath );
            var buildFile = deserialiser.Deserialize<BuildFile>( text );
            return buildFile;
        }
    }
}
