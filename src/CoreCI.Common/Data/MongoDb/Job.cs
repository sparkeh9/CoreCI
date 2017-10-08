namespace CoreCI.Common.Data.MongoDb
{
    using System.Collections.Generic;
    using Common.Newtonsoft.Json;
    using Models;
    using Models.Jobs;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;

    public class Job : IMongoEntity
    {
        public string CollectionName => "Jobs";

        [ BsonId ]
        [ JsonConverter( typeof( MongoObjectIdConverter ) ) ]
        public ObjectId Id { get; set; }

        [ JsonConverter( typeof( MongoObjectIdConverter ) ) ]
        public ObjectId Project { get; set; }

        public string BuildAgentToken { get; set; }
        public BuildEnvironmentOs Environment { get; set; }
        public BuildMode BuildMode { get; set; } = BuildMode.Native;
        public JobStatus JobStatus { get; set; }
        public VcsJob Data { get; set; }
        public IList<JobProgressDto> Logs { get; set; } = new List<JobProgressDto>();
    }
}
