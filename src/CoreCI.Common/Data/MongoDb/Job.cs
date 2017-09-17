namespace CoreCI.Common.Data.MongoDb
{
    using Common.Newtonsoft.Json;
    using Models;
    using Models.Jobs;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using ObjectId = MongoDB.Bson.ObjectId;

    public class Job : IMongoEntity
    {
        public string CollectionName => "Jobs";

        [ BsonId ]
        [ JsonConverter( typeof( MongoObjectIdConverter ) ) ]
        public ObjectId Id { get; set; }

        [ JsonConverter( typeof( MongoObjectIdConverter ) ) ]
        public ObjectId Project { get; set; }

        public BuildEnvironment Environment { get; set; }
        public JobStatus JobStatus { get; set; }
        public VcsJob Data { get; set; }
    }
}
