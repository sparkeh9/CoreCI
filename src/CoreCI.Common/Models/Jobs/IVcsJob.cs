namespace CoreCI.Common.Models.Jobs
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public interface IVcsJob
    {
        [ JsonConverter( typeof( StringEnumConverter ) ) ]
        VcsType VcsType { get; }
    }
}
