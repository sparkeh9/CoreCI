namespace CoreCI.Common.Models.Solutions
{
    using System.Collections.Generic;
    using Common.Newtonsoft.Json;
    using MongoDB.Bson;
    using Newtonsoft.Json;
    using Projects;

    public class SolutionDetailDto
    {
        [ JsonConverter( typeof( MongoObjectIdConverter ) ) ]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<ProjectDto> Projects { get; set; }
        public Dictionary<string, Link> Links { get; set; }
    }
}
