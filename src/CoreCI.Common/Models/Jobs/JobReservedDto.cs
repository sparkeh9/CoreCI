namespace CoreCI.Common.Models.Jobs
{
    using System;
    using Common.Newtonsoft.Json;
    using MongoDB.Bson;
    using Newtonsoft.Json;

    public class JobReservedDto
    {
        [ JsonConverter( typeof( MongoObjectIdConverter ) ) ]
        public ObjectId JobId { get; set; }

        public string BuildAgentToken { get; set; }
    }
}
