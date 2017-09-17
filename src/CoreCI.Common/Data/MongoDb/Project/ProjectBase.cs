namespace CoreCI.Common.Data.MongoDb.Project
{
    using Common.Newtonsoft.Json;
    using Models;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;

    [ BsonDiscriminator( nameof( ProjectBase ), RootClass = true ) ]
    public abstract class ProjectBase : IMongoEntity
    {
        public string CollectionName => "Projects";

        [ BsonId ]
        [ JsonConverter( typeof( MongoObjectIdConverter ) ) ]
        public ObjectId Id { get; set; }

        public BuildEnvironment Environment { get; set; }
        public virtual VcsType VcsType { get; }
    }
}
