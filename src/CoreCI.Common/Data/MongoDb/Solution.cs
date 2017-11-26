namespace CoreCI.Common.Data.MongoDb
{
    using Common.Newtonsoft.Json;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;

    public class Solution : IMongoEntity
    {
        public string CollectionName => "Solutions";

        [ BsonId ]
        [ JsonConverter( typeof( MongoObjectIdConverter ) ) ]
        public ObjectId Id { get; set; }

        public string Name { get; set; }
    }
}
