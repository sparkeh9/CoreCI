﻿namespace CoreCI.Common.Models.Jobs
{
    using System.Collections.Generic;
    using Common.Newtonsoft.Json;
    using MongoDB.Bson;
    using Newtonsoft.Json;

    public class JobDto
    {
        [ JsonConverter( typeof( MongoObjectIdConverter ) ) ]
        public ObjectId JobId { get; set; }

        public BuildEnvironmentOs Environment { get; set; }
        public BuildMode BuildMode { get; set; }
        public Dictionary<string, Link> Links { get; set; }

        [ JsonConverter( typeof( VcsJobConverter ) ) ]
        public VcsJob Data { get; set; }

        public string BuildAgentToken { get; set; }
    }
}
