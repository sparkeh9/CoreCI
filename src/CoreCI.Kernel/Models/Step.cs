namespace CoreCI.Kernel.Models
{
    using System.Collections.Generic;

    public class Step
    {
        public string Name { get; set; }
        public Image Image { get; set; }
        public IDictionary<string, string> EnvironmentVariables { get; set; }
        public IEnumerable<VolumeBinding> Volumes { get; set; }
        public IEnumerable<string> Commands { get; set; }
    }

    internal static class StepExtensions
    {
        public static Dictionary<string, object> ExtractVolumes( this Step operand, string workspacePath )
        {
            var dictionary = new Dictionary<string, object>
            {
                { "/workspace", new { } }
            };
////
//            if ( operand?.Volumes != null )
//            {
//                foreach ( var volume in operand?.Volumes )
//                {
//                    if ( volume.LocalPath.IsNullOrWhiteSpace() )
//                        throw new LocalPathMissingException($"Local path missing for binding: {volume.RemotePath} in step {operand.Name}");
//
//                    dictionary.Add( volume.LocalPath, new { } );
//                }
//            }

            return dictionary;
        }

        public static IEnumerable<string> ExtractBindings( this Step operand, string workspacePath )
        {
            yield return $"{workspacePath}:/workspace";

            if ( operand?.Volumes != null )
            {
                foreach ( var volume in operand?.Volumes )
                {
                    yield return $"{volume.LocalPath}:{volume.RemotePath}";
                }
            }
        }
    }
}