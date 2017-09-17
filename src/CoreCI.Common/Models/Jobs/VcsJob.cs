namespace CoreCI.Common.Models.Jobs
{
    using Data.MongoDb.Project;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [ BsonDiscriminator( nameof( ProjectBase ), RootClass = true ) ]
    public abstract class VcsJob
    {
        [ JsonConverter( typeof( StringEnumConverter ) ) ]
        public virtual VcsType VcsType { get; } = VcsType.Git;
    }
}
